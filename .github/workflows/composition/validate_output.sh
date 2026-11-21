#!/bin/bash
set -euo pipefail

VERSION="${VERSION_NEXT:-}"
VERSION_NS="${VERSION_NOSUFFIX:-}"
if [ -z "$VERSION_NS" ]; then
    VERSION_NS="$VERSION" # IF NOT PREVIEW SAY THE VERSION NO SUFFIX, IF PREVIEW YOU GET A x.x.x HERE
fi
MAJOR="${VERSION_NS%%.*}"
MAJOR_ZERO_ZERO="${MAJOR}.0.0" # FOR .NET PROJECT ASSEMBLY VERSION.

BRANCH="${BRANCH:-}"
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
)

if [ "$BRANCH" = "main-dev" ]; then
  ALLOWED_FILES+=("/tmp/artifacts/output/preview.txt")
else
  ALLOWED_FILES+=("/tmp/artifacts/output/stable.txt")
fi

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
