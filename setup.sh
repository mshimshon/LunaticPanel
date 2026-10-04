#!/usr/bin/env bash
set -e

# 1. Enforce root privileges
if [ "$EUID" -ne 0 ]; then
  echo "Error: This script must be run as root." >&2
  exit 1
fi
BAREBONE_ONLY=false

# Parse command line arguments
for arg in "$@"; do
  if [ "$arg" == "--barebone-only" ]; then
    BAREBONE_ONLY=true
    break
  fi
done
# ==========================================
# CONFIGURATION
# ==========================================
REPO="mshimshon/LunaticPanel"
SETUP_DIR="/tmp/lunaticpanel_setup"

# Add or modify your pre-installed plugins here
pre_installed_plugins=(
  "plugin1"
  "plugin2"
  "plugin3"
)
# ==========================================

# 2. Clear and recreate the setup directory
rm -rf "$SETUP_DIR"
mkdir -p "$SETUP_DIR"

echo "Checking the latest LunaticPanel release..."
# Fetch the latest release tag (e.g., v1.2.3)
LATEST_TAG=$(curl -s "https://github.com{REPO}/releases/latest" | grep '"tag_name":' | sed -E 's/.*"([^"]+)".*/\1/')

if [ -z "$LATEST_TAG" ]; then 
  echo "Error: Could not fetch latest release tag from GitHub." >&2
  exit 1
fi

# 3. Extract pure version string (removes leading 'v' if present, e.g., v1.2.3 -> 1.2.3)
VERSION="${LATEST_TAG#v}"

echo "Detected Release Tag: $LATEST_TAG"
echo "Extracted Version: $VERSION"

# 4. Download main payload asset
MAIN_ARCHIVE="lunaticpanel.${VERSION}.tar.gz"
echo "Downloading main payload: ${MAIN_ARCHIVE}..."
curl -sL "https://github.com{REPO}/releases/download/${LATEST_TAG}/${MAIN_ARCHIVE}" -o "${SETUP_DIR}/${MAIN_ARCHIVE}"

# Download the install runner script
curl -sL "https://github.com{REPO}/releases/download/${LATEST_TAG}/install.sh" -o "${SETUP_DIR}/install.sh"
chmod +x "${SETUP_DIR}/install.sh"

# 5. Conditional plugin downloads
if [ "$BAREBONE_ONLY" = false ]; then
  echo "Downloading pre-installed plugins..."
  for plugin in "${pre_installed_plugins[@]}"; do
    PLUGIN_FILE="${plugin}.${VERSION}.lpkg"
    echo " -> Downloading ${PLUGIN_FILE}..."
    curl -sL "https://github.com{REPO}/releases/download/${LATEST_TAG}/${PLUGIN_FILE}" -o "${SETUP_DIR}/${PLUGIN_FILE}"
  done
else
  echo "--barebone-only flag detected. Skipping plugin downloads."
fi

# 6. Hand off execution to the downloaded installer
echo "Handing off execution to the installer..."
"${SETUP_DIR}/install.sh" "$VERSION" "${SETUP_DIR}/${MAIN_ARCHIVE}"

# 7. Clean up workspace
rm -rf "$SETUP_DIR"
echo "Setup complete!"
