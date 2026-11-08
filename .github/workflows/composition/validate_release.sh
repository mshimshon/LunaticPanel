set -euo pipefail
VERSION_NEXT="${VERSION_NEXT:-}"
VERSION_NEXT_PREVIEW="${VERSION_NEXT_PREVIEW:-}"
BRANCH="${BRANCH:-}"

if [ "$BRANCH" = "main-dev" ]; then
  if [ -z "$VERSION_NEXT_PREVIEW" ]; then
    echo "::error::main-dev build but VERSION_NEXT_PREVIEW is empty" >&2
    exit 1
  fi
  VERSION="$VERSION_NEXT_PREVIEW"
else
  if [ -z "$VERSION_NEXT" ]; then
    echo "::error::VERSION_NEXT is empty" >&2
    exit 1
  fi
  VERSION="$VERSION_NEXT"
fi

echo "Releasing version: $VERSION"
OUTPUT="/tmp/artifacts/output"

echo "Checking directory: $OUTPUT"

# Define array of expected filenames
ALLOWED_FILES=(
  "LunaticPanel.Core.Analyzer.$VERSION.nupkg"
  "LunaticPanel.Core.Abstraction.$VERSION.nupkg"
  "LunaticPanel.Core.PluginValidator.$VERSION.nupkg"
  "LunaticPanel.Core.$VERSION.nupkg"
  "LunaticPanel.Core.Extensions.$VERSION.nupkg"
  "LunaticPanel.Core.Utils.Abstraction.$VERSION.nupkg"
  "LunaticPanel.Core.Utils.$VERSION.nupkg"
  "LunaticPanel.Package.Server.$VERSION.nupkg"
  "LunaticPanel.Package.Tool.$VERSION.nupkg"
  "LunaticPanel.PackageManager.Keys.$VERSION.nupkg"
  "LunaticPanel.Engine.Keys.$VERSION.nupkg"
  "GameHost.Keys.$VERSION.nupkg"
  "_.$VERSION.md"
  "build.env"
)
# Fail immediately if a required file is missing
for file in "${ALLOWED_FILES[@]}"; do
  if [ ! -f "$OUTPUT/$file" ]; then
    echo "❌ Error: Required file missing -> $file"
    echo "🚨 Validation failed!"
    exit 1
  fi
done

# Optional: List contents only if everything up to this point passed
ls "$OUTPUT"

echo "✅ Validation passed successfully! Match complete."
