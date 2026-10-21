$ErrorActionPreference = "Stop"

$tokenPattern = [regex]::new("(?i)\b(break|feat|fix|note|wip)\b\s*:")
$releasePrefixes = @("break:", "feat:", "fix:", "note:")

function Invoke-GitCommand {
    param(
        [Parameter(Mandatory)]
        [string[]] $Arguments
    )

    Write-Host "[DEBUG] Running command: git $($Arguments -join ' ')"

    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = "git"
    $startInfo.UseShellExecute = $false
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true

    if ($PSVersionTable.PSVersion.Major -ge 7) {
        foreach ($argument in $Arguments) {
            $startInfo.ArgumentList.Add($argument)
        }
    }
    else {
        $quotedArguments = foreach ($argument in $Arguments) {
            $quoted = [System.Text.StringBuilder]::new()
            [void] $quoted.Append('"')
            $backslashCount = 0

            foreach ($character in $argument.ToCharArray()) {
                if ($character -eq '\') {
                    $backslashCount++
                }
                elseif ($character -eq '"') {
                    [void] $quoted.Append('\' * (2 * $backslashCount + 1))
                    [void] $quoted.Append('"')
                    $backslashCount = 0
                }
                else {
                    if ($backslashCount -gt 0) {
                        [void] $quoted.Append('\' * $backslashCount)
                        $backslashCount = 0
                    }
                    [void] $quoted.Append($character)
                }
            }

            if ($backslashCount -gt 0) {
                [void] $quoted.Append('\' * (2 * $backslashCount))
            }
            [void] $quoted.Append('"')
            $quoted.ToString()
        }

        $startInfo.Arguments = $quotedArguments -join ' '
    }

    $process = [System.Diagnostics.Process]::new()
    $process.StartInfo = $startInfo

    try {
        [void] $process.Start()
        $stdoutTask = $process.StandardOutput.ReadToEndAsync()
        $stderrTask = $process.StandardError.ReadToEndAsync()
        $process.WaitForExit()
        $stdout = $stdoutTask.GetAwaiter().GetResult()
        $stderr = $stderrTask.GetAwaiter().GetResult()

        if ($process.ExitCode -ne 0) {
            [Console]::Error.WriteLine("[ERROR] Command failed with exit code $($process.ExitCode)")
            [Console]::Error.WriteLine($stderr)
            exit $process.ExitCode
        }

        return $stdout.Trim()
    }
    finally {
        $process.Dispose()
    }
}

function Set-GitHubOutput {
    param(
        [Parameter(Mandatory)]
        [string] $Name,
        [Parameter(Mandatory)]
        [string] $Value
    )

    $githubOutput = $env:GITHUB_OUTPUT
    if (-not [string]::IsNullOrEmpty($githubOutput)) {
        Write-Host "[DEBUG] Setting GitHub Output -> ${Name}=${Value}"

        [System.IO.File]::AppendAllText(
            $githubOutput,
            "${Name}<<EOF`n${Value}`nEOF`n",
            [System.Text.UTF8Encoding]::new($false)
        )
    }
    else {
        Write-Host "[DEBUG] Standard Output Toggle -> ${Name}=${Value}"
    }
}

function Get-CommitEntries {
    param(
        [Parameter(Mandatory)]
        [string] $Message
    )

    $regexMatches = @($tokenPattern.Matches($Message))
    $entries = [System.Collections.Generic.List[string]]::new()

    for ($index = 0; $index -lt $regexMatches.Count; $index++) {
        $match = $regexMatches[$index]
        $tag = $match.Groups[1].Value.ToLowerInvariant()
        $start = $match.Index + $match.Length

        if ($index + 1 -lt $regexMatches.Count) {
            $end = $regexMatches[$index + 1].Index
        }
        else {
            $end = $Message.Length
        }

        $value = $Message.Substring($start, $end - $start).Trim()
        if (-not [string]::IsNullOrEmpty($value)) {
            $entries.Add("${tag}: ${value}")
        }
    }

    return $entries.ToArray()
}

function Format-DebugList {
    param(
        [Parameter(Mandatory)]
        [AllowEmptyCollection()]
        [string[]] $Values
    )

    $formattedValues = foreach ($value in $Values) {
        $escaped = $value.Replace("\", "\\").Replace("'", "\'").Replace("`r", "\r").Replace("`n", "\n").Replace("`t", "\t")
        "'$escaped'"
    }

    return "[$($formattedValues -join ', ')]"
}

function Test-CommitHasWip {
    param(
        [Parameter(Mandatory)]
        [System.Collections.IDictionary] $Commit
    )

    foreach ($entry in @(Get-CommitEntries -Message $Commit.message)) {
        if ($entry.StartsWith("wip:", [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
    }

    return $false
}

function Get-BranchCommits {
    $raw = [string] (Invoke-GitCommand -Arguments @(
        "log",
        "origin/main..HEAD",
        "--no-merges",
        "--format=%H%x1f%B%x1e"
    ))

    $commits = [System.Collections.Generic.List[object]]::new()
    $blocks = @($raw.Split([char] 0x1e) | ForEach-Object { $_.Trim() } | Where-Object { $_ })

    Write-Host "[DEBUG] Loaded $($blocks.Count) raw commits from branch tree window."

    foreach ($block in $blocks) {
        $separatorIndex = $block.IndexOf([char] 0x1f)
        if ($separatorIndex -lt 0) {
            continue
        }

        $commits.Add(@{
            sha = $block.Substring(0, $separatorIndex).Trim()
            message = $block.Substring($separatorIndex + 1).Trim()
        })
    }

    return $commits.ToArray()
}

Write-Host "[DEBUG] --- STARTING CONTEXT SQUASH EVALUATION ---"
$commits = @(Get-BranchCommits)

if ($commits.Count -eq 0) {
    Write-Host "[DEBUG] No commits found in branch history block. Exiting."
    Set-GitHubOutput -Name "squashed_ready" -Value "false"
    exit 0
}

Write-Host "[DEBUG] Commit array index tracking topology (Newest -> Oldest):"
for ($index = 0; $index -lt $commits.Count; $index++) {
    $commit = $commits[$index]
    $firstLine = ($commit.message -split "`n", 2)[0]
    Write-Host "   [$index] SHA: $($commit.sha.Substring(0, [Math]::Min(7, $commit.sha.Length))) | Message Summary: $firstLine"
}

$head = $commits[0]
Write-Host "[DEBUG] Target HEAD evaluation commit -> SHA: $($head.sha.Substring(0, [Math]::Min(7, $head.sha.Length)))"

$headEntries = @(Get-CommitEntries -Message $head.message)
Write-Host "[DEBUG] Extracted tags from HEAD: $(Format-DebugList -Values $headEntries)"

$hasWipTag = $false
foreach ($entry in $headEntries) {
    if ($entry.StartsWith("wip:", [System.StringComparison]::OrdinalIgnoreCase)) {
        $hasWipTag = $true
        break
    }
}

if ($hasWipTag) {
    Write-Host "[DEBUG] HEAD contains a WIP tag. Stopping execution so it cannot be pushed."
    Set-GitHubOutput -Name "squashed_ready" -Value "false"
    exit 1
}

$hasReleaseTag = $false
foreach ($entry in $headEntries) {
    foreach ($prefix in $releasePrefixes) {
        if ($entry.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)) {
            $hasReleaseTag = $true
            break
        }
    }

    if ($hasReleaseTag) {
        break
    }
}
Write-Host "[DEBUG] Does HEAD contain a release action tag? -> $($hasReleaseTag.ToString().ToLowerInvariant())"

if (-not $hasReleaseTag) {
    Write-Host "[DEBUG] HEAD does not contain a release indicator. Stopping execution path."
    Set-GitHubOutput -Name "squashed_ready" -Value "false"
    exit 1
}

$firstWipCommit = $null
$firstWipIndex = $null

Write-Host "[DEBUG] Searching for cutoff WIP line boundary moving Oldest -> Newest..."
for ($index = $commits.Count - 1; $index -ge 0; $index--) {
    $commit = $commits[$index]
    $isWip = Test-CommitHasWip -Commit $commit
    $firstLine = ($commit.message -split "`n", 2)[0]
    $shortSha = $commit.sha.Substring(0, [Math]::Min(7, $commit.sha.Length))
    Write-Host "   Checking index [$index] $shortSha -> Has WIP? $($isWip.ToString().ToLowerInvariant()) ('$firstLine')"

    if ($isWip) {
        $firstWipCommit = $commit.sha
        $firstWipIndex = $index
        Write-Host "[DEBUG] Match confirmed! Cutoff anchored at array index [$index], SHA: $shortSha"
        break
    }
}

if ($null -eq $firstWipCommit) {
    Write-Host "[DEBUG] No WIP tag encountered on this branch segment. Nothing to squash."
    Set-GitHubOutput -Name "squashed_ready" -Value "false"
    exit 1
}

Write-Host "[DEBUG] Slicing commit array up to index boundary: [: $firstWipIndex + 1]"
$cycleCommits = @($commits | Select-Object -First ($firstWipIndex + 1))

Write-Host "[DEBUG] Commits inside the active squash capture window ($($cycleCommits.Count) total):"
foreach ($commit in $cycleCommits) {
    $shortSha = $commit.sha.Substring(0, [Math]::Min(7, $commit.sha.Length))
    Write-Host "   -> Including SHA: $shortSha in rebuild processing loop."
}

$rebuiltEntries = [System.Collections.Generic.List[string]]::new()
foreach ($commit in $cycleCommits) {
    $entries = @(Get-CommitEntries -Message $commit.message)
    $shortSha = $commit.sha.Substring(0, [Math]::Min(7, $commit.sha.Length))
    Write-Host "[DEBUG] Processing SHA $shortSha, found entries: $(Format-DebugList -Values $entries)"

    foreach ($entry in $entries) {
        $keepEntry = $false
        foreach ($prefix in $releasePrefixes) {
            if ($entry.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase)) {
                $keepEntry = $true
                break
            }
        }

        if ($keepEntry) {
            Write-Host "   [KEEP] Appending release tag: $entry"
            $rebuiltEntries.Add($entry)
        }
        else {
            Write-Host "   [DROP] Skipping text segment tag: $entry"
        }
    }
}

if ($rebuiltEntries.Count -eq 0) {
    Write-Host "[DEBUG] Empty compilation string array block context. Aborting build process."
    Set-GitHubOutput -Name "squashed_ready" -Value "false"
    exit 1
}

$rebuiltMessage = $rebuiltEntries -join "`n"
$commitFile = if ($env:OS -eq "Windows_NT") {
    Join-Path $env:TEMP "commit.txt"
}
else {
    "/tmp/commit.txt"
}

Write-Host "[DEBUG] Attempting filesystem write sequence -> $commitFile"
try {
    [System.IO.File]::WriteAllText(
        $commitFile,
        $rebuiltMessage + "`n",
        [System.Text.UTF8Encoding]::new($false)
    )
    Write-Host "[DEBUG] Filesystem file lock write action finished cleanly."
}
catch {
    [Console]::Error.WriteLine("[ERROR] Failed filesystem call writing map payload: $_")
    exit 1
}

Write-Host "==== REBUILT MESSAGE WRITTEN TO $commitFile ===="
Write-Host $rebuiltMessage
Write-Host "===================================================="

$firstWipParents = @(
    (Invoke-GitCommand -Arguments @(
        "rev-list",
        "--parents",
        "-n",
        "1",
        $firstWipCommit
    )) -split "\s+"
)

if ($firstWipParents.Count -gt 1) {
    Invoke-GitCommand -Arguments @("reset", "--soft", $firstWipParents[1]) | Out-Null
}
else {
    Invoke-GitCommand -Arguments @("update-ref", "-d", "HEAD") | Out-Null
}

Invoke-GitCommand -Arguments @(
    "commit",
    "--allow-empty",
    "-F",
    $commitFile
) | Out-Null

Set-GitHubOutput -Name "squashed_ready" -Value "false"