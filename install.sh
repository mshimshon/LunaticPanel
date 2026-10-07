#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

# Ensure the script is being run as root
if [ "$EUID" -ne 0 ]; then
    echo "Error: This script must be run as root." >&2
    exit 1
fi

SERVICE_NAME="lunaticpanel"
SERVICE_FILE="/etc/systemd/system/${SERVICE_NAME}.service"
SETUP_DIR="/tmp/lunaticpanel_setup"
TARGET_DIR="/srv/lunaticpanel"
PRE_INSTALLED_DIR="${TARGET_DIR}/pre_installed"

# --- SYSTEM SETTINGS & RUNTIME VARIABILITY ---
# Change this variable as versions progress in the future (e.g., "11.0", "12.0")
TARGET_DOTNET_VERSION="10.0"

# --- SERVICE CONFIGURATION ---
DESCRIPTION="LunaticPanel Service"
WORKING_DIR="${TARGET_DIR}"

EXEC_START="/usr/bin/dotnet"
STARTUP_PARAMETERS="${TARGET_DIR}/LunaticPanel.Hybrid.Web.dll --urls \"http://localhost:5001;\""

DEPENDS_ON=() 
ENVIRONMENT=()
# ------------------------------------------------------

echo "=== Starting LunaticPanel Install Script ==="

# --- AUTOMATED OFFICIAL MICROSOFT SCRIPTED INSTALL ---
echo "Checking if .NET ${TARGET_DOTNET_VERSION} ASP.NET Core Runtime is available..."

# Check if dotnet exists globally and has the required runtime version loaded
if command -v dotnet &> /dev/null && dotnet --list-runtimes | grep -q "Microsoft.AspNetCore.App ${TARGET_DOTNET_VERSION}\."; then
    echo "Validated: .NET ${TARGET_DOTNET_VERSION} ASP.NET Core Runtime is already available."
else
    echo ".NET ${TARGET_DOTNET_VERSION} runtime missing or not detected. Running official Microsoft installer..."
    
    # 1. Install standard extraction tools if missing
    apt-get update -y
    apt-get install -y wget tar ca-certificates

    # 2. Download official Microsoft .NET installation script
    wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh

    # 3. Run script targeting a global /usr/share/dotnet directory
    #    This downloads only the lightweight aspnetcore runtime stack
    /tmp/dotnet-install.sh --channel "${TARGET_DOTNET_VERSION}" --runtime aspnetcore --install-dir /usr/share/dotnet
    
    # 4. Create standard global symlink if it doesn't already exist
    if [ ! -f /usr/bin/dotnet ]; then
        ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
    fi

    # Clean up installer script
    rm /tmp/dotnet-install.sh

    # 5. Final runtime verification
    if ! dotnet --list-runtimes | grep -q "Microsoft.AspNetCore.App ${TARGET_DOTNET_VERSION}\."; then
        echo "Error: Microsoft install script completed but .NET ${TARGET_DOTNET_VERSION} was not registered correctly." >&2
        exit 1
    fi
    echo ".NET ${TARGET_DOTNET_VERSION} standard installation successfully verified!"
fi


# --- STEP 3: ARCHIVE REGEX RESOLUTION ---
echo "Searching for archive in ${SETUP_DIR}..."
TAR_MATCH=$(find "$SETUP_DIR" -maxdepth 1 -regextype posix-extended -regex ".*/lunaticpanel\.[0-9]+\.[0-9]+\.[0-9]+\.tar\.gz" | head -n 1)

if [ -z "$TAR_MATCH" ]; then
    echo "Error: No matching lunaticpanel.x.x.x.tar.gz file found in ${SETUP_DIR}" >&2
    exit 1
fi
echo "Found archive: ${TAR_MATCH}"


# --- STEPS 1 & 2: LIFECYCLE MANAGEMENT & SERVICE TEMPLATING ---
if systemctl list-unit-files | grep -q "^${SERVICE_NAME}.service"; then
    echo "Stopping existing ${SERVICE_NAME} service..."
    systemctl stop "${SERVICE_NAME}" || true
else
    echo "Service ${SERVICE_NAME} not found. Constructing service file..."
    cat <<EOF > "$SERVICE_FILE"
[Unit]
Description=${DESCRIPTION}
After=network.target ${DEPENDS_ON[*]}
$( [ ${#DEPENDS_ON[@]} -gt 0 ] && echo "Requires=${DEPENDS_ON[*]}" )

[Service]
Type=simple
WorkingDirectory=${WORKING_DIR}
ExecStart=${EXEC_START} ${STARTUP_PARAMETERS}
Restart=no
$(for item in "${ENVIRONMENT[@]}"; do echo "Environment=${item}"; done)
LogsDirectory=${SERVICE_NAME}

[Install]
WantedBy=multi-user.target
EOF

    echo "Template successfully created at ${SERVICE_FILE}. Reloading systemd..."
    systemctl daemon-reload
fi


# --- STEPS 4 & 5: DIRECTORY SANITIZATION & EXTRACTION ---
if [ -d "$TARGET_DIR" ]; then
    echo "Directory ${TARGET_DIR} exists. Clearing contents..."
    rm -rf "${TARGET_DIR:?}"/*
else
    echo "Creating directory ${TARGET_DIR}..."
    mkdir -p "$TARGET_DIR"
fi
chmod 755 "$TARGET_DIR"

echo "Extracting archive to ${TARGET_DIR}..."
tar -xzf "$TAR_MATCH" -C "$TARGET_DIR" --strip-components=0


# --- STEPS 6 & 7: PRE_INSTALLED EXTENSION STORAGE MIGRATION ---
echo "Checking for .lpkg files..."
mkdir -p "$PRE_INSTALLED_DIR"

if compgen -G "${SETUP_DIR}/*.lpkg" > /dev/null; then
    echo "Moving .lpkg packages to ${PRE_INSTALLED_DIR}..."
    mv "$SETUP_DIR"/*.lpkg "$PRE_INSTALLED_DIR"/
else
    echo "No .lpkg files found to move."
fi


# --- STEP 8: BOOT LIFECYCLE KICKOFF ---
echo "Starting ${SERVICE_NAME} service..."
systemctl start "$SERVICE_NAME"

echo "=== Setup Completed Successfully ==="
