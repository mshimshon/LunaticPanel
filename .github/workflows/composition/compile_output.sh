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

OUT="/tmp/artifacts/output";
BUILD="/tmp/artifacts/build";
BUILD_TOOL="/tmp/artifacts/tool";
BUILD_WEB="/tmp/artifacts/web";
BUILD_LUNAPKG="/tmp/artifacts/lunapackage";
mkdir -p "$OUT"
mkdir -p "$BUILD"
mkdir -p "$BUILD_TOOL"
mkdir -p "$BUILD_WEB"
mkdir -p "$BUILD_LUNAPKG"

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
    -o "$OUT" \
    /p:InformationalVersion=$VERSION /p:Version=$VERSION /p:FileVersion=$VERSION_NS /p:AssemblyVersion=$MAJOR_ZERO_ZERO
done

dotnet publish "./tools/LunaticPanel.Package.Tool/LunaticPanel.Package.Tool.csproj" -r linux-x64 -c Release -o "$BUILD_TOOL" /p:InformationalVersion=$VERSION /p:Version=$VERSION /p:FileVersion=$VERSION_NS /p:AssemblyVersion=$MAJOR_ZERO_ZERO

dotnet publish ./src/LunaticPanel.Hybrid/LunaticPanel.Hybrid.Web/LunaticPanel.Hybrid.Web.csproj -c Release -r linux-x64 --self-contained false -o $BUILD_WEB \
  /p:InformationalVersion=$VERSION /p:Version=$VERSION /p:FileVersion=$VERSION_NS /p:AssemblyVersion=$MAJOR_ZERO_ZERO


dotnet publish ./tools/LunaticPanel.Package.LocalServer/LunaticPanel.Package.LocalServer.csproj -c Release -r linux-x64 --self-contained false -o $BUILD_LUNAPKG \
  /p:InformationalVersion=$VERSION /p:Version=$VERSION /p:FileVersion=$VERSION_NS /p:AssemblyVersion=$MAJOR_ZERO_ZERO

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
    /p:InformationalVersion=$VERSION /p:Version=$VERSION /p:FileVersion=$VERSION_NS /p:AssemblyVersion=$MAJOR_ZERO_ZERO


  /tmp/artifacts/tool/LunaticPanel.Package.Tool \
    pack \
    --input "$TARGET_BUILD_DIR" \
    --output "$OUT" \
    --version $VERSION

  EXPECTED_PKG="$OUT/${PROJECT_NAME}.${VERSION}.lpkg"

  if [ -z "$EXPECTED_PKG" ] || [ ! -s "$EXPECTED_PKG" ]; then
    ls -R $OUT
    echo "❌ Error: Packed file for ${PROJECT_NAME} version ${VERSION} was not generated or is empty!"
    exit 1
  else
    echo "✅ Verified: $(basename "$EXPECTED_PKG") exists and is valid."
  fi
done

echo "$VERSION" > $BUILD_WEB/.version
echo "$VERSION" > $BUILD_LUNAPKG/.version
echo "$VERSION" > $OUT/.version
tar -czf "$OUT/lunaticpanel_$VERSION.tar.gz" -C $BUILD_WEB .
tar -czf "$OUT/lunapackage_server_$VERSION.tar.gz" -C $BUILD_LUNAPKG .
tar -czf "$OUT/lunaticpanel_plugins_$VERSION.tar.gz" -C $OUT/ $(cd $OUT && ls *.lpkg)

if [ -f /tmp/artifacts/output/preview.txt ]; then
    cp /tmp/artifacts/output/preview.txt /tmp/artifacts/cdn/
fi
if [ "$BRANCH" = "main-dev" ]; then
  echo "$VERSION" > $OUT/preview.txt
else
  echo "$VERSION" > $OUT/stable.txt
fi


