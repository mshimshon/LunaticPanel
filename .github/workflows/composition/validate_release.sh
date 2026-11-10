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

echo "Checking directory: /tmp/artifacts/output"

# Define array of expected filenames
ALLOWED_FILES=(
  "/tmp/artifacts/output/LunaticPanel.Core.Analyzer.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Core.Abstraction.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Core.PluginValidator.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Core.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Core.Extensions.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Core.Utils.Abstraction.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Core.Utils.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Package.Server.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Package.Tool.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.PackageManager.Keys.$VERSION.nupkg"
  "/tmp/artifacts/output/LunaticPanel.Engine.Keys.$VERSION.nupkg"
  "/tmp/artifacts/output/_.$VERSION.md"
  "/tmp/artifacts/output/build.env"
  "/tmp/artifacts/output/lunaticpanel_$VERSION.tar.gz"
  "/tmp/artifacts/output/lunapackage_server_$VERSION.tar.gz"
  "/tmp/artifacts/output/lunaticpanel_plugins_$VERSION.tar.gz"
  "/tmp/artifacts/output/stable.txt"
)
# Fail immediately if a required file is missing
for file in "${ALLOWED_FILES[@]}"; do
  if [ ! -f "$file" ]; then
    echo "❌ Error: Required file missing -> $file"
    echo "🚨 Validation failed!"
    exit 1
  fi
done

# Optional: List contents only if everything up to this point passed
ls "/tmp/artifacts/output"

echo "✅ Validation passed successfully! Match complete."
