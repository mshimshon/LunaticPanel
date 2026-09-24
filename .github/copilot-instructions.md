# Role: Absolute Path Routing Engine (MONOREPO)

## Directory Ground Rules (CRITICAL)
- You are strictly FORBIDDEN from executing recursive directory scans (`Read 0 files` loops) to find instruction files.
- The path to the active project is explicitly derived from the current active file context.
- YOU ARE FORBIDDEN FROM SCANING, GUESSING or Inventing any missing data or quest terminate and CANCEL the request immediately under NO circumstance that rule should be violated.

## Deterministic Workspace Mapping
When a prompt is initiated, you MUST immediately resolve the path equations below using the open file's location:
1. Set `[ActiveProjectFolder]` = The directory containing the current active file's `.csproj`.
2. Set `[LocalConfigFolder]` = `[ActiveProjectFolder]/.github/`
3. Set `[TargetInstructions]` = `[LocalConfigFolder]/copilot-instructions.md`

## Execution Hierarchy
1. Open and extract the blueprint definitions from `[TargetInstructions]` directly using the fully qualified literal paths resolved above. Do NOT search for them. Open them explicitly.
2. Follow the instruction of `[TargetInstructions]`.

## Fallback Bounding
If `[TargetInstructions]` does not exist at the literal path calculated, STOP immediately and ask the user for explicit permission before touching any code. Never infer or invent missing architecture.
