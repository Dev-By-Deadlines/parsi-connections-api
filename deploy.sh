#!/bin/bash
set -e

# Configuration
VPS_ALIAS="tehran"                 # your SSH config host name
DEPLOY_DIR="/root/connections-api" # where the app lives on the VPS
SERVICE_NAME="connections"         # systemd service name

echo "🔨 Publishing self‑contained app for linux‑x64..."
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish

echo "🚀 Syncing to VPS..."
rsync -avz --delete -e "ssh" ./publish/ $VPS_ALIAS:$DEPLOY_DIR/

echo "🔄 Restarting service..."
ssh $VPS_ALIAS "sudo systemctl restart $SERVICE_NAME"

echo "✅ Deploy complete! API is live."
