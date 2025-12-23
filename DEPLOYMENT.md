# MaskMText Deployment Guide

## Option 1: Individual Installation (APPLOAD Method)

### Installation Steps:
1. Copy `MaskMText.dll` to a permanent location on your computer
   - Recommended: `C:\Program Files\CustomPlugins\MaskMText\`
2. Open Civil 3D
3. Type `APPLOAD` and press Enter
4. Click **Contents** button in the Startup Suite section
5. Click **Add**
6. Browse to the DLL location and select it
7. Click **Close**
8. Restart Civil 3D - the plugin will now load automatically

### Uninstallation:
1. Type `APPLOAD` in Civil 3D
2. Click **Contents**
3. Select `MaskMText.dll` from the list
4. Click **Remove**

---

## Option 2: Network Deployment (Recommended for Teams)

### Setup for IT/CAD Manager:

1. **Create network share structure:**
   ```
   \\YourServer\Civil3D\Plugins\
   └── MaskMText\
       ├── MaskMText.dll
       └── README.md
   ```

2. **Copy the acad.lsp file to your support path:**
   - Typical location: `\\YourServer\Civil3D\Support\`
   - Or user profile: `%APPDATA%\Autodesk\C3D 2023\enu\Support\`

3. **Update acad.lsp with your network path:**
   ```lisp
   (defun s::startup ()
     (command "._NETLOAD" "\\\\YourServer\\Civil3D\\Plugins\\MaskMText\\MaskMText.dll")
     (princ "\nMaskMText plugin loaded - Type BB to run.")
     (princ)
   )
   ```

4. **Add support path in Civil 3D (if needed):**
   - Type `OPTIONS` → Files tab → Support File Search Path
   - Add: `\\YourServer\Civil3D\Support\`

### For Users:
- No action needed! Plugin loads automatically when they open Civil 3D
- If network path is unavailable, Civil 3D will start normally without the plugin

---

## Option 3: AutoCAD Bundle (.bundle) - Advanced

Create a bundle package for easier distribution:

### Bundle Structure:
```
MaskMText.bundle\
└── Contents\
    ├── PackageContents.xml
    └── Windows\
        └── 2023\
            └── MaskMText.dll
```

### Installation:
1. Copy the entire `MaskMText.bundle` folder to:
   - `C:\ProgramData\Autodesk\ApplicationPlugins\`
2. Restart Civil 3D
3. Plugin loads automatically

---

## Option 4: Silent Deployment via Script

For IT departments deploying to multiple workstations:

### PowerShell Script (deploy-maskmtext.ps1):
```powershell
# Copy DLL to standard location
$targetPath = "C:\ProgramData\Autodesk\Civil3DPlugins\MaskMText"
New-Item -ItemType Directory -Force -Path $targetPath
Copy-Item "MaskMText.dll" -Destination $targetPath

# Add to APPLOAD startup (per user)
$regPath = "HKCU:\Software\Autodesk\AutoCAD\R24.2\ACAD-6001:409\Applications\MaskMText"
New-Item -Path $regPath -Force
Set-ItemProperty -Path $regPath -Name "LOADCTRLS" -Value 2
Set-ItemProperty -Path $regPath -Name "LOADER" -Value "$targetPath\MaskMText.dll"

Write-Host "MaskMText installed successfully"
```

---

## Sharing with External Users

### Simple Distribution Package:
1. Zip the following files together:
   - `MaskMText.dll`
   - `README.md`
   - `DEPLOYMENT.md` (this file)

2. Share via:
   - Email
   - SharePoint/OneDrive
   - GitHub releases
   - Internal file share

### Installation Instructions for Recipients:
"Extract the zip file and follow Option 1 (APPLOAD Method) above"

---

## Troubleshooting

### Plugin doesn't load:
- Check if .NET Framework 4.8 is installed
- Verify DLL is not blocked (Right-click → Properties → Unblock)
- Check Civil 3D version matches (2023)
- Run `NETLOAD` manually to see error messages

### Command not found:
- Verify DLL loaded: Check Civil 3D command line for load message
- Check for command name conflicts
- Try `NETLOAD` to reload

### Network path issues:
- Ensure all users have Read access to network share
- Test UNC path: `\\server\path\` (not mapped drives)
- Consider offline scenario handling

---

## Version Management

### Updating to new versions:
1. Close all instances of Civil 3D
2. Replace the DLL file
3. Reopen Civil 3D

### Version tracking:
- Tag releases in GitHub
- Update README.md with version notes
- Consider adding version info in DLL properties

---

## Security Considerations

- DLLs should be digitally signed (for production use)
- Store in write-protected network locations
- Use Group Policy to deploy/update
- Test in sandbox environment first

---

## Support

For issues or questions:
- GitHub: https://github.com/Xuan27/maskmtext
- Contact your CAD administrator
