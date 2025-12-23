# PowerShell script to create AutoCAD .bundle package
# Run this after building the project in Release mode

param(
    [string]$OutputPath = ".\bundle"
)

Write-Host "Creating MaskMText.bundle package..." -ForegroundColor Green

# Define paths
$bundleName = "MaskMText.bundle"
$bundlePath = Join-Path $OutputPath $bundleName
$contentsPath = Join-Path $bundlePath "Contents"
$windowsPath = Join-Path $contentsPath "Windows\2023"

# Create directory structure
Write-Host "Creating bundle directory structure..."
New-Item -ItemType Directory -Force -Path $windowsPath | Out-Null

# Copy DLL
$dllSource = ".\bin\Release\MaskMText.dll"
if (Test-Path $dllSource) {
    Write-Host "Copying DLL..."
    Copy-Item $dllSource -Destination $windowsPath -Force
} else {
    Write-Error "DLL not found at $dllSource. Build the project first!"
    exit 1
}

# Copy PackageContents.xml
Write-Host "Copying PackageContents.xml..."
Copy-Item ".\PackageContents.xml" -Destination $bundlePath -Force

# Copy documentation
Write-Host "Copying documentation..."
Copy-Item ".\README.md" -Destination $contentsPath -Force
Copy-Item ".\DEPLOYMENT.md" -Destination $contentsPath -Force

Write-Host "`nBundle created successfully at: $bundlePath" -ForegroundColor Green
Write-Host "`nTo install:" -ForegroundColor Yellow
Write-Host "  Copy the entire '$bundleName' folder to:" -ForegroundColor Yellow
Write-Host "  C:\ProgramData\Autodesk\ApplicationPlugins\" -ForegroundColor Cyan
Write-Host "`nThen restart Civil 3D." -ForegroundColor Yellow

# Optional: Create zip for distribution
$zipPath = Join-Path $OutputPath "MaskMText-Bundle.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Write-Host "`nCreating zip file for distribution..."
Compress-Archive -Path $bundlePath -DestinationPath $zipPath -Force
Write-Host "Distribution zip created: $zipPath" -ForegroundColor Green
