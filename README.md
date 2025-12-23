# BBMASK - MaskMText for Civil 3D

A Civil 3D plugin that efficiently manages text masking for MText and MultiLeader objects.

## Features

- **Quick Masking**: Single command to mask multiple text objects
- **Batch Processing**: Select multiple MText or MultiLeader objects at once
- **Customizable Settings**: Adjust mask size (offset) and color
- **Persistent Settings**: Settings remain across command invocations during session

## Installation

1. Build the project in Visual Studio (Release configuration)
2. Copy `MaskMText.dll` to a known location
3. In Civil 3D, use `NETLOAD` command to load the DLL
4. Optionally, add to startup suite for automatic loading

## Usage

### Command: `BBMASK`

When you run `BBMASK`, you'll be prompted with options:

#### Option 1: Mask (Default)
1. Select "Mask" or press Enter
2. Select MText and/or MultiLeader objects
3. Press Enter to apply masks
4. Objects will have background masks applied with current settings

#### Option 2: Settings
1. Select "Settings"
2. Enter desired mask offset factor (e.g., 1.5 = 150% of text height)
3. Enter mask color index (0-256, where 7 = white/black auto)
4. Settings are saved for the current session

### Mask Settings

- **Mask Offset**: Controls the size of the mask border around text
  - Default: 1.5 (50% padding)
  - Range: Any positive number (1.0 = no padding, 2.0 = 100% padding)

- **Mask Color**: AutoCAD Color Index for the mask background
  - Default: 7 (White/Black - adapts to background)
  - Common values: 0 = ByBlock, 7 = White, 255 = Background color

## System Requirements

- AutoCAD Civil 3D 2024 or later
- .NET Framework 4.8
- Windows x64

## Build Instructions

1. Update DLL reference paths in `MaskMText.csproj` to match your Civil 3D installation
2. Open solution in Visual Studio 2022 or later
3. Build in Release mode
4. DLL will be in `bin/Release` folder

## License

Free to use and modify for personal and commercial projects.

## Author

Created for efficient text masking workflows in Civil 3D.
