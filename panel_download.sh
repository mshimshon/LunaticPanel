#!/usr/bin/env bash
set -e

# 1. Enforce root privileges
if [ "$EUID" -ne 0 ]; then
  echo "Error: This script must be run as root." >&2
  exit 1
fi

# ==========================================
# CONFIGURATION
# ==========================================
REPO="mshimshon/LunaticPanel"
SETUP_DIR="/tmp/lunaticpanel_setup"
TARGET_DOTNET_VERSION="10.0"

SERVICE_NAME="lunaticpanel"
TARGET_DIR="/srv/lunaticpanel"
WORKING_DIR="${TARGET_DIR}"

# Parse optional arguments
# Usage: ./deploy.sh [version] [plugin_id;plugin_id;plugin_id]
SPECIFIED_VERSION="$2"
PLUGINS_ARG="$1"

# 2. Clear and recreate the setup directory hierarchy
rm -rf "$SETUP_DIR"
mkdir -p "$SETUP_DIR/preinstalled"
mkdir -p "$SETUP_DIR/extracted"
# =================

# ==========================================
# GITHUB RELEASE RESOLUTION & VALIDATION
# ==========================================
if [ -n "$SPECIFIED_VERSION" ]; then
  # Normalize to ensure the API call uses the proper 'v' tag syntax
  if [[ "$SPECIFIED_VERSION" == v* ]]; then
    LATEST_TAG="$SPECIFIED_VERSION"
  else
    LATEST_TAG="v${SPECIFIED_VERSION}"
  fi
  
  echo "Validating specified version tag on GitHub: $LATEST_TAG..."
  RELEASE_JSON=$(curl -sSL "https://api.github.com/repos/${REPO}/releases/tags/${LATEST_TAG}")
  
  # Check for 404 response safely without letting a grep exit code crash set -e
  if echo "$RELEASE_JSON" | grep -q '"message":[[:space:]]*"Not Found"'; then
    echo "Error: Specified version '$SPECIFIED_VERSION' does not exist as a release on GitHub." >&2
    exit 1
  fi
  
  # Crucial: Extract the pure version number for file downloads (removes 'v')
  VERSION="${LATEST_TAG#v}"
else
  echo "Checking the latest LunaticPanel release..."
  RELEASE_JSON=$(curl -sSL "https://api.github.com/repos/${REPO}/releases/latest")

  # Extract the tag safely without a grep pipeline crash
  LATEST_TAG=$(echo "$RELEASE_JSON" | sed -nE 's/.*"tag_name": *"[vV]?([^"]+)".*/v\1/p')

  if [ -z "$LATEST_TAG" ]; then 
    echo "Error: Could not fetch latest release tag from GitHub." >&2
    exit 1
  fi
  
  # Extract the pure version number for file downloads (removes 'v')
  VERSION="${LATEST_TAG#v}"
fi


# Extract pure version string (removes leading 'v' if present, e.g., v1.2.3 -> 1.2.3)
VERSION="${LATEST_TAG#v}"

echo "Target Release Tag: $LATEST_TAG"
echo "Extracted Version: $VERSION"

# Define asset structures
MAIN_ARCHIVE="lunaticpanel.${VERSION}.tar.gz"

# ==========================================
# PHASE 1: PRE-FLIGHT ASSET EXISTENCE CHECK
# ==========================================
echo "🔍 Running pre-flight verification on GitHub release assets..."

# 1. Verify main application payload exists
if ! echo "$RELEASE_JSON" | grep -q "\"name\": \"${MAIN_ARCHIVE}\""; then
  echo "Error: Pre-flight check failed! Main archive '${MAIN_ARCHIVE}' is missing from GitHub release metadata." >&2
  exit 1
fi
echo "✅ Verified: Core payload ($MAIN_ARCHIVE) exists on GitHub."

# 2. Verify all listed plugin assets exist
if [ -n "$PLUGINS_ARG" ]; then
  IFS=';' read -ra PLUGINS_ARRAY <<< "$PLUGINS_ARG"
  for PLUGIN_ID in "${PLUGINS_ARRAY[@]}"; do
    PLUGIN_ID=$(echo "$PLUGIN_ID" | xargs) # Trim whitespace
    [ -z "$PLUGIN_ID" ] && continue
    
    EXPECTED_LPKG="${PLUGIN_ID}.${VERSION}.lpkg"
    
    if ! echo "$RELEASE_JSON" | grep -q "\"name\": \"${EXPECTED_LPKG}\""; then
        echo "Plugin asset ($EXPECTED_LPKG) does not exists on Official GitHub."
    else
         echo "✅ Verified: Plugin asset ($EXPECTED_LPKG) exists on GitHub."
    fi
  done
fi

echo "🚀 All assets verified on GitHub. Proceeding to downloads..."


# Fixed URL string: added '$' and missing '/'
DOWNLOAD_TARGET="https://github.com/${REPO}/releases/download/${LATEST_TAG}/${MAIN_ARCHIVE}" 
echo "📥 Downloading main payload from: ${DOWNLOAD_TARGET}"
curl -sSL "$DOWNLOAD_TARGET" -o "${SETUP_DIR}/bin.tar.gz"

if [ ! -s "${SETUP_DIR}/bin.tar.gz" ]; then
  echo "Error: Downloaded core archive is empty or failed." >&2
  exit 1
fi

# Download plugins if specified
if [ -n "$PLUGINS_ARG" ]; then
  for PLUGIN_ID in "${PLUGINS_ARRAY[@]}"; do
    PLUGIN_ID=$(echo "$PLUGIN_ID" | xargs)
    [ -z "$PLUGIN_ID" ] && continue
    
    EXPECTED_LPKG="${PLUGIN_ID}.${VERSION}.lpkg"
    
    # Fixed URL string here as well: added '$' and missing '/'
    PLUGIN_DOWNLOAD_URL="https://github.com/${REPO}/releases/download/${LATEST_TAG}/${EXPECTED_LPKG}"
    
    echo "📥 Downloading plugin: ${EXPECTED_LPKG}..."
        wget --quiet --tries=3 "$PLUGIN_DOWNLOAD_URL" -O "${SETUP_DIR}/preinstalled/${EXPECTED_LPKG}"

    if [ ! -s "${SETUP_DIR}/preinstalled/${EXPECTED_LPKG}" ]; then
      echo "Error: Downloaded plugin archive ${EXPECTED_LPKG} failed or is empty." >&2
      exit 1
    fi
  done
fi


echo "🎉 Download successful"