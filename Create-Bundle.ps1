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

# Copy DLL - check multiple possible locations
$dllSource = $null
$possiblePaths = @(
    ".\bin\x64\Release\MaskMText.dll",
    ".\bin\Release\MaskMText.dll",
    ".\bin\Release\net48\MaskMText.dll"
)

foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $dllSource = $path
        break
    }
}

if ($dllSource) {
    Write-Host "Copying DLL from: $dllSource"
    Copy-Item $dllSource -Destination $windowsPath -Force
} else {
    Write-Error "DLL not found in any expected location. Build the project first!"
    Write-Host "Checked locations:" -ForegroundColor Yellow
    foreach ($path in $possiblePaths) {
        Write-Host "  - $path" -ForegroundColor Yellow
    }
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
