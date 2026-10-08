sda#!/usr/bin/env bash
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
SETUP_DIR="/tmp/lunaticpanel_lpkgserver_setup"
SERVICE_NAME="lpkg_localserver"
SERVICE_FILE="/etc/systemd/system/${SERVICE_NAME}.service"
TARGET_DIR="/srv/lunaticpanel_lpkg_localserver"
TARGET_DOTNET_VERSION="10.0"
DESCRIPTION="LunaticPanel Lightweight package serving."
WORKING_DIR="${TARGET_DIR}"

EXEC_START="/usr/bin/dotnet"
STARTUP_PARAMETERS="${TARGET_DIR}/LunaticPanel.Package.LocalServer.dll --urls \"http://localhost:5002;\""

DEPENDS_ON=() 
ENVIRONMENT=()
# 2. Clear and recreate the setup directory
rm -rf "$SETUP_DIR"
mkdir -p "$SETUP_DIR"


echo "Checking the latest LunaticPanel release..."
LATEST_TAG=$(curl -s "https://api.github.com/repos/${REPO}/releases/latest" \
  | grep '"tag_name"' \
  | sed -E 's/.*"([^"]+)".*/\1/')
echo "LATEST_TAG = $LATEST_TAG"
if [ -z "$LATEST_TAG" ]; then 
  echo "Error: Could not fetch latest release tag from GitHub." >&2
  exit 1
fi

# 3. Extract pure version string (removes leading 'v' if present, e.g., v1.2.3 -> 1.2.3)
VERSION="${LATEST_TAG#v}"

echo "Detected Release Tag: $LATEST_TAG"
echo "Extracted Version: $VERSION"

# 4. Download main payload asset lpkg-server-lite.0.0.29.tar.gz
MAIN_ARCHIVE="lpkg-server-lite.${VERSION}.tar.gz"
DOWNLOAD_TARGET="https://github.com/${REPO}/releases/download/${LATEST_TAG}/${MAIN_ARCHIVE}" 
echo "Downloading main payload: ${DOWNLOAD_TARGET}..."
curl -sL $DOWNLOAD_TARGET -o "${SETUP_DIR}/${MAIN_ARCHIVE}"
ls $SETUP_DIR
if [ -d "$WORKING_DIR" ]; then
    rm -rf "$WORKING_DIR"/*
fi
mkdir -p "$WORKING_DIR"
tar -xzf "${SETUP_DIR}/${MAIN_ARCHIVE}" -C $WORKING_DIR
ls "$WORKING_DIR"

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

systemctl start "${SERVICE_NAME}" || true
systemctl status "${SERVICE_NAME}" || true

echo "Delaying for Krestel"
sleep 1.5

echo "Streaming payload via pipeline..."

# 1. Loop and pipe the request until Kestrel actually serves the JSON payload
MAX_ATTEMPTS=10
for ((i=1; i<=MAX_ATTEMPTS; i++)); do
  # We pipe curl directly into a variable check. 
  # If curl returns nothing, the loop yields and retries.
  RESPONSE=$(curl -s http://localhost:5002/lpkg/versions | cat)
  
  if [ -n "$RESPONSE" ]; then
    break
  fi
  
  echo "Server port is open but payload is empty (.NET is still initializing). Retrying in 1s ($i/$MAX_ATTEMPTS)..."
  sleep 1
done

# 2. Strict validation check on the final piped result
if [ -z "$RESPONSE" ]; then
  echo "Error: Server connected but consistently returned an empty payload after $MAX_ATTEMPTS seconds." >&2
  echo "--- Fetching Systemd Application Logs ---"
  journalctl -u "${SERVICE_NAME}" -n 25
  exit 1
fi

echo "Available API Versions: $RESPONSE"