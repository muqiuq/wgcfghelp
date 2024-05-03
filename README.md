# WgCfgHelp - Wireguard Config Helper

Wireguard Config Helper is a lightweight CLI tool designed to make Wireguard VPN setup easy and efficient, enabling bulk configuration creation for deployments of any size.

## ⭐ Features
 - Manage RoadWarrior WireGuard configuration in a single YAML file
 - Bulk client access configuration generator
 - Generate QR Codes images alongside client config files
 - Outputs *[WgQuick](https://www.man7.org/linux/man-pages/man8/wg-quick.8.html) config files* or *MirkoTik commands*

## 🧸 Motivation
 - I needed to create multiple client access configuration files.
 - I was looking for a tool that consisted of a single binary (with no dependencies) and ran on any operating system.
 - Previously, I used Python scripts to accomplish this, but I wanted a tool I could share with other IT colleagues.

## 🚀 Upcoming features
 - Sync configuration with MikroTik Router
 - Generate site configuration from existing WgQuick configuration file

## 🚧 Still In development

This project is currently in development and **not yet ready for production use**. If you are excited about what we're building and want to contribute, we warmly welcome anyone to **join our effort**! 

## 🔧 Quick start

### Requirements
 - [WireGuard](https://www.wireguard.com/install/)
   - On macOS: Using homebrew
   - On Windows: Using Installer
   - On Linux: Using package manager

### Steps

 1. Download latest release

Linux / MacOS (bash / zsh)
```bash
# Linux arm64
curl -L https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.linux-arm64 -o wgcfghelp && chmod +x wgcfghelp
```
```bash
# Linux x64
curl -L https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.linux-x64 -o wgcfghelp && chmod +x wgcfghelp
```
```bash
# MacOs x64 (works for arm64 too on macOS Sonoma 14.x)
curl -L https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.osx-x64  -o wgcfghelp && chmod +x wgcfghelp
```

Windows (PowerShell)
```powershell
# Windows x64
$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -Uri "https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.x64.exe" -OutFile "wgcfghelp.exe"; $ProgressPreference = 'Continue';
```

```powershell
# Windows arm64
$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -Uri "https://github.com/muqiuq/wgcfghelp/releases/latest/download/WgCfgHelp.CLI.arm64.exe" -OutFile "wgcfghelp.exe"; $ProgressPreference = 'Continue';
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

## ⌨️ Usage
```bash
./wgcfghelp --help

# or for help of specific verb
./wgcfghelp gen-site --help
```

## Copyright

"WireGuard" and the "WireGuard" logo are registered trademarks of Jason A. Donenfeld.
