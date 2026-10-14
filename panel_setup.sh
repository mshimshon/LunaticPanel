#!/usr/bin/env bash
set -e

# 1. Enforce root privileges
if [ "$EUID" -ne 0 ]; then
  echo "Error: This script must be run as root." >&2
  exit 1
fi

SPECIFIED_VERSION="$2"
PLUGINS_ARG="$1"

# ==========================================
# CONFIGURATION
# ==========================================
REPO="mshimshon/LunaticPanel"
SETUP_DIR="/tmp/lunaticpanel_setup"
TARGET_DOTNET_VERSION="10.0"

SERVICE_NAME="lunaticpanel"
SERVICE_FILE="/etc/systemd/system/${SERVICE_NAME}.service"
TARGET_DIR="/srv/lunaticpanel"
DESCRIPTION="LunaticPanel Lightweight package serving."
WORKING_DIR="${TARGET_DIR}"
EXEC_START="/usr/bin/dotnet"
STARTUP_PARAMETERS="${TARGET_DIR}/LunaticPanel.Hybrid.Web.dll --urls \"http://*:10001;\""
DEPENDS_ON=() 
ENVIRONMENT=()

# 2. Clear and recreate the setup directory
rm -rf "$SETUP_DIR"
mkdir -p "$SETUP_DIR"

apt update -y



# --- AUTOMATED OFFICIAL MICROSOFT SCRIPTED INSTALL ---
echo "Checking if .NET ${TARGET_DOTNET_VERSION} ASP.NET Core Runtime is available..."

# Check if dotnet exists globally and has the required runtime version loaded
if command -v dotnet &> /dev/null && dotnet --list-runtimes | grep -q "Microsoft.AspNetCore.App ${TARGET_DOTNET_VERSION}\."; then
    echo "Validated: .NET ${TARGET_DOTNET_VERSION} ASP.NET Core Runtime is already available."
else
    echo ".NET ${TARGET_DOTNET_VERSION} runtime missing or not detected. Running official Microsoft installer..."
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

# Check and install libicu-dev
if ! dpkg -s libicu-dev &> /dev/null; then
    echo "libicu-dev is not installed. Installing now..."
    apt install -y libicu-dev
else
    echo "libicu-dev is already installed. Skipping."
fi


if systemctl list-unit-files | grep -q "^${SERVICE_NAME}.service"; then
    echo "${SERVICE_NAME} Exist.............. OK!"
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
OOMScoreAdjust=-900
CPUWeight=1000
CPUQuota=100%
MemoryMin=514M
MemoryHigh=4G

$(for item in "${ENVIRONMENT[@]}"; do echo "Environment=${item}"; done)
LogsDirectory=${SERVICE_NAME}

[Install]
WantedBy=multi-user.target
EOF
    echo "Template successfully created at ${SERVICE_FILE}. Reloading systemd..."
    systemctl daemon-reload
fi

# ==========================================
# BOOTSTRAP & EXECUTION PIPELINE
# ==========================================

# --- PHASE 1: DOWNLOAD ENGINE ---
SCRIPT_DOWNLOAD="panel_download.sh"

if [ ! -f "./${SCRIPT_DOWNLOAD}" ]; then
  echo "⚠️ Local execution script ./${SCRIPT_DOWNLOAD} is missing!"
  echo "📥 Downloading fresh script from the main branch of ${REPO}..."
  
  wget -qO "./${SCRIPT_DOWNLOAD}" "https://raw.githubusercontent.com/${REPO}/main/${SCRIPT_DOWNLOAD}"

  if [ ! -s "./${SCRIPT_DOWNLOAD}" ] || grep -q "404: Not Found" "./${SCRIPT_DOWNLOAD}"; then
    echo "Error: Failed to fetch ${SCRIPT_DOWNLOAD} from GitHub main branch." >&2
    rm -f "./${SCRIPT_DOWNLOAD}"
    exit 1
  fi
  chmod +x "./${SCRIPT_DOWNLOAD}"
  echo "✅ ${SCRIPT_DOWNLOAD} successfully retrieved and made executable."
fi

echo "⚙️ Executing download sequence..."
/bin/bash "./${SCRIPT_DOWNLOAD}" "$@" || exit 1


# --- PHASE 2: INSTALL ENGINE (Triggers only if Phase 1 succeeds) ---
SCRIPT_INSTALL="panel_install.sh"

if [ ! -f "./${SCRIPT_INSTALL}" ]; then
  echo "⚠️ Local execution script ./${SCRIPT_INSTALL} is missing!"
  echo "📥 Downloading fresh script from the main branch of ${REPO}..."
  
  wget -qO "./${SCRIPT_INSTALL}" "https://raw.githubusercontent.com/${REPO}/main/${SCRIPT_INSTALL}"

  if [ ! -s "./${SCRIPT_INSTALL}" ] || grep -q "404: Not Found" "./${SCRIPT_INSTALL}"; then
    echo "Error: Failed to fetch ${SCRIPT_INSTALL} from GitHub main branch." >&2
    rm -f "./${SCRIPT_INSTALL}"
    exit 1
  fi
  chmod +x "./${SCRIPT_INSTALL}"
  echo "✅ ${SCRIPT_INSTALL} successfully retrieved and made executable."
fi

echo "⚙️ Executing installation sequence..."
/bin/bash "./${SCRIPT_INSTALL}" "$@" || exit 1

echo "🎉 Entire pipeline execution completed successfully!"