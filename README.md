# WgCfgHelp - Wireguard Config Helper

Wireguard Config Helper is a lightweight CLI tool designed to make Wireguard VPN setup easy and efficient, enabling bulk configuration creation for deployments of any size.

## ⭐ Features
 - Create and use site config file
 - Generate single or multiple client access configuration
 - Generate QR Codes images alongside client config files
 - *Upcomming: Generate MirkoTik Commands*


## 🧸 Motivation
 - I needed to create multiple client access configuration files.
 - I was looking for a tool that consisted of a single binary and ran on any operating system.
 - Previously, I used Python scripts to accomplish this, but I wanted a tool I could share with other IT colleagues.

## 🔧 Quick start

 1. Download latest release

Linux / MacOS (bash / zsh)
```bash
# Linux arm64
curl -L https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.linux-arm64 -o wgcfghelp && chmod +x wgcfghelp

# Linux x64
curl -L https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.linux-x64 -o wgcfghelp && chmod +x wgcfghelp

# MacOs x64 (works for arm64 too on macOS Sonoma 14.x)
curl -L https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.osx-x64  -o wgcfghelp && chmod +x wgcfghelp
```

Windows (PowerShell)
```powershell
# Windows x64
Invoke-WebRequest -Uri "https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.x64.exe" -OutFile "wgcfghelp.exe"

# Windows x64
Invoke-WebRequest -Uri "https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.arm64.exe" -OutFile "wgcfghelp.exe"
```

 2. Create new site (this will create a yaml file CompanyXYRoadwarrior.yaml)
```bash
./wgcfghelp gen-site CompanyXYRoadwarrior "192.168.9.0/24,192.168.10.0/24" example.com:13328
```

 3. Create a folder to store the client config files
```bash
mkdir configfiles
```

 4. Create 5 new clients with configuration file and QRCodes
```bash
# this will output 192.168.9.2.conf and 192.168.9.2.png in the configfiles folder
./wgcfghelp gen-client ./CompanyXYRoadwarrior.yaml -o -b ./configfiles 192.168.9.2 --qrcode -n 5
```

 5. Create server configuration 
```bash
# this will output 192.168.9.1_24.conf
./wgcfghelp gen-server ./CompanyXYRoadwarrior.yaml 192.168.9.1 -o
```
 6. Done. The files can now be distributed