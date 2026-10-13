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
# bin.tar.gz
TARGET_DOTNET_VERSION="10.0"

SERVICE_NAME="lunaticpanel"
SERVICE_FILE="/etc/systemd/system/${SERVICE_NAME}.service"
TARGET_DIR="/srv/LunaticPanel"
DESCRIPTION="LunaticPanel Lightweight package serving."
WORKING_DIR="${TARGET_DIR}"
EXEC_START="/usr/bin/dotnet"
STARTUP_PARAMETERS="${TARGET_DIR}/LunaticPanel.Hybrid.Web.dll --urls \"http://localhost:10001;\""
DEPENDS_ON=() 
ENVIRONMENT=()
if systemctl list-unit-files | grep -q "^${SERVICE_NAME}.service"; then
    echo "${SERVICE_NAME} Does not Exist"
    exit 1
else
    systemctl stop "${SERVICE_NAME}" || true
fi

mkdir -p "$WORKING_DIR"

mkdir -p "$SETUP_DIR/extracted"
mkdir -p "$SETUP_DIR/preinstalled"
rm -rf "$SETUP_DIR/extracted"/{*,.[!.]*,..?*} 2>/dev/null || true

# Verify the core DLL exists in the target directory
if [ ! -f "${SETUP_DIR}/bin.tar.gz"  ]; then
    echo "Error: Required file bin.tar.gz install file not present, run setup first was not found in ${TARGET_DIR}."
    exit 1
fi

tar -xzf "${SETUP_DIR}/bin.tar.gz" -C "$SETUP_DIR/extracted" || exit 1
rm -rf "$WORKING_DIR"/{*,.[!.]*,..?*} 2>/dev/null || true
cp -r "$SETUP_DIR/extracted"/. "$WORKING_DIR"/ || exit 1
cp -r "$SETUP_DIR/preinstalled"/*.lpkg "$WORKING_DIR/plugins_preinstalled"/ || exit 1

rm -rf "$SETUP_DIR"/{*,.[!.]*,..?*} 2>/dev/null || true
systemctl enable "${SERVICE_NAME}" || true
systemctl start "${SERVICE_NAME}" || true
