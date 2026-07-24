try {
    # --- CONFIGURATION ---
    $PropsFileName = "Directory.Packages.props"
    $PropsPath = Join-Path -Path $PSScriptRoot -ChildPath $PropsFileName
    
    # Target output location relative to the script root folder
    $OutputPath = [System.IO.Path]::GetFullPath((Join-Path -Path $PSScriptRoot -ChildPath "../../../nupkgs/"))

    # --- 1. READ FILE & EXTRACT VERSION ---
    if (-not (Test-Path $PropsPath)) {
        throw "Critical Error: ${PropsFileName} not found at ${PropsPath}"
    }

    $fileContent = Get-Content -Path $PropsPath -Raw
    
    # Target your custom <GlobalVersion> tag explicitly
    $regexPattern = '(?i)(?<=<GlobalVersion>\s*)[0-9]+\.[0-9]+\.[0-9]+(?=\s*</GlobalVersion>)'

    if (-not ($fileContent -match $regexPattern)) {
        throw "Critical Error: Could not find <GlobalVersion> tag with a 3-part version in ${PropsFileName}."
    }

    # Extract current version safely
    $currentVersion = [regex]::Match($fileContent, $regexPattern).Value.Trim()
    Write-Host "Current global version detected: ${currentVersion}" -ForegroundColor Cyan

    # --- 2. DYNAMICALLY FIND ELIGIBLE PROJECTS ---
    Write-Host "Scanning directories for eligible .csproj files..." -ForegroundColor Gray
    
    # Recursively find all .csproj files from the root path
    $allCsprojs = Get-ChildItem -Path $PSScriptRoot -Filter "*.csproj" -Recurse
    $ProjectFilesToPack = @()

    foreach ($csproj in $allCsprojs) {
        $csprojContent = Get-Content -Path $csproj.FullName -Raw
        
        # Check if the file contains the target element trigger name
        if ($csprojContent -like '*Name="CopyNuPkg"*') {
            $ProjectFilesToPack += $csproj
            Write-Host "Found matching project: $($csproj.Name)" -ForegroundColor Green
        }
    }

    if ($ProjectFilesToPack.Count -eq 0) {
        throw "Script Stopped: No .csproj files were found containing Name=`"CopyNuPkg`"."
    }

    # --- 3. CALCULATE INCREMENTED VERSION ---
    $versionParts = $currentVersion.Split('.')
    
    # Extract indices safely using valid array assignment syntax
    [int]$major = [int]$versionParts[0]
    [int]$minor = [int]$versionParts[1]
    [int]$build = [int]$versionParts[2]

    $build++
    if ($build -gt 999) {
        $build = 0
        $minor++
    }
    if ($minor -gt 999) {
        $minor = 0
        $major++
    }

    $newVersion = "$major.$minor.$build"
    Write-Host "Target next version: ${newVersion}" -ForegroundColor Cyan

    # Ensure output target directory exists
    if (-not (Test-Path $OutputPath)) {
        New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null
        Write-Host "Created output directory: $OutputPath" -ForegroundColor Gray
    }

    # --- 4. EXECUTE DOTNET PACK ---
    foreach ($csproj in $ProjectFilesToPack) {
        Write-Host "Packing project: $($csproj.Name) with version ${newVersion}..." -ForegroundColor Yellow
        
        # Pack command targeted precisely at the matched file paths
        dotnet pack $csproj.FullName --configuration Debug --output $OutputPath /p:Version=${newVersion} /p:PackageVersion=${newVersion}
        
        # IMMEDIATELY HALT EXEcUTION ON FAILURE
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to pack project: $($csproj.Name). Halting pipeline immediately."
        }
    }

    # --- 5. UPDATE FILE ON SUCCESS ---
    $updatedContent = [regex]::Replace($fileContent, $regexPattern, $newVersion)
    Set-Content -Path $PropsPath -Value $updatedContent -NoNewline
    Write-Host "`nSuccessfully updated ${PropsFileName} to version ${newVersion}!" -ForegroundColor Green
    Write-Host "Packages saved to: $OutputPath" -ForegroundColor Green

} catch {
    Write-Error $_
    exit 1
} finally {
    Write-Host "`n----------------------------------------" -ForegroundColor Gray
    Write-Host "Execution finished." -ForegroundColor Gray
}
