#!/bin/bash
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
OUT="/tmp/artifacts/output";
BUILD="/tmp/artifacts/build";
mkdir -p "$OUT"
mkdir -p "$BUILD"

KEY_PROJECTS=(
  "./src/LunaticPanel.Core.Analyzer/LunaticPanel.Core.Analyzer.csproj"
  "./src/LunaticPanel.Core.Abstraction/LunaticPanel.Core.Abstraction.csproj"
  "./src/LunaticPanel.Core.PluginValidator/LunaticPanel.Core.PluginValidator.csproj"
  "./src/LunaticPanel.Core/LunaticPanel.Core.csproj"
  "./src/LunaticPanel.Core.Extensions/LunaticPanel.Core.Extensions.csproj"
  "./src/LunaticPanel.Core.Utils.Abstraction/LunaticPanel.Core.Utils.Abstraction.csproj"
  "./src/LunaticPanel.Core.Utils/LunaticPanel.Core.Utils.csproj"
  "./tools/LunaticPanel.Package.Server/LunaticPanel.Package.Server.csproj"
  "./tools/LunaticPanel.Package.Tool/LunaticPanel.Package.Tool.csproj"
  "./tools/LunaticPanel.DebugTool/LunaticPanel.DebugTool.csproj"
  "./plugins/PackageManager/LunaticPanel.PackageManager.Keys/LunaticPanel.PackageManager.Keys.csproj"
  "./src/LunaticPanel.Engine.Keys/LunaticPanel.Engine.Keys.csproj"
  # add more Keys projects here
)

for KEY in "${KEY_PROJECTS[@]}"; do
  echo "🔄 Packing: $KEY."
  dotnet pack \
    "$KEY" \
    -c Release \
    -o "/tmp/artifacts/output" \
    /p:Version=$VERSION
done


PLUGINS=(
  "./plugins/PackageManager/LunaticPanel.PackageManager/LunaticPanel.PackageManager.csproj"
)

for KEY in "${PLUGINS[@]}"; do
  FILE_NAME=$(basename "$KEY")
  PROJECT_NAME="${FILE_NAME%.csproj}"

  TARGET_BUILD_DIR="/tmp/artifacts/plugins_build/$PROJECT_NAME"
  mkdir -p "$TARGET_BUILD_DIR"

  dotnet publish \
    "$KEY" \
    -r linux-x64 \
    -c Release \
    -o "$TARGET_BUILD_DIR" \
    /p:InformationalVersion=$VERSION \
    /p:Version=$VERSION \
    /p:FileVersion=$VERSION_NEXT \
    /p:AssemblyVersion=$VERSION_NEXT

  /tmp/artifacts/tool/LunaticPanel.Package.Tool \
    pack \
    --input "$TARGET_BUILD_DIR" \
    --output "/tmp/artifacts/output" \
    --version $VERSION

  EXPECTED_PKG="/tmp/artifacts/output/${PROJECT_NAME}.${VERSION}.lpkg"

  if [ -z "$EXPECTED_PKG" ] || [ ! -s "$EXPECTED_PKG" ]; then
    ls -R /tmp/artifacts/output
    echo "❌ Error: Packed file for ${PROJECT_NAME} version ${VERSION} was not generated or is empty!"
    exit 1
  else
    echo "✅ Verified: $(basename "$EXPECTED_PKG") exists and is valid."
  fi
done

dotnet test LunaticPanel.slnx --no-build --configuration Release --verbosity normal /p:TreatWarningsAsErrors=false
