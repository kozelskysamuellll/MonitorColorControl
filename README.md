# Monitor Color Control

A powerful Windows desktop application for advanced monitor color settings beyond standard NVIDIA Control Panel options.

## Features

- **Advanced Color Controls:**
  - Gamma (0.1 to 5.0, up to 10.0 in Extreme Mode)
  - Brightness (-100 to +300)
  - Contrast (0 to 300)
  - Saturation (0 to 500%, up to 1000% in Extreme Mode)
  - Digital Vibrance (0 to 500%, up to 1000% in Extreme Mode)
  - RGB Channel Controls
  - Black Level and White Level adjustment
  - Color Temperature control
  - Hue Shift

- **Profile Management:**
  - Create custom presets
  - Save/Load profiles as JSON
  - Auto-apply settings on startup
  - Import/Export functionality

- **Advanced Features:**
  - Dark-themed modern UI
  - Real-time slider preview
  - Multiple monitor support
  - System tray integration
  - Global hotkeys for profile switching
  - Undo/Reset functionality
  - Extreme Mode for maximum customization

## Requirements

- Windows 10 or Windows 11
- .NET 8 Runtime
- No NVIDIA Control Panel dependency

## Quick Start

### Build from Source

```bash
git clone https://github.com/kozelskysamuellll/MonitorColorControl.git
cd MonitorColorControl
dotnet restore
dotnet build -c Release
```

### Run the Application

```bash
dotnet run
```

### Generate Standalone Executable

```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true
```

Output: `bin/Release/net8.0-windows/publish/MonitorColorControl.exe`

## Project Structure

```
MonitorColorControl/
├── Models/
│   ├── ColorSettings.cs         # Color adjustment settings
│   ├── ColorProfile.cs          # Profile data model
│   └── Constants.cs             # Application constants
├── Services/
│   ├── GammaRampService.cs      # Windows Gamma Ramp API
│   ├── ColorTransformService.cs # Color calculations
│   ├── ProfileManager.cs        # Profile persistence
│   ├── HotkeyManager.cs         # Global hotkey handling
│   └── MonitorService.cs        # Multi-monitor support
├── Views/
│   ├── MainWindow.xaml          # Main UI
│   ├── MainWindow.xaml.cs       # Code-behind
│   └── PreviewPanel.xaml        # Preview component
├── Converters/
│   ├── SliderValueConverter.cs
│   └── ColorConverter.cs
├── Resources/
│   └── Dark.xaml                # Dark theme
├── App.xaml                      # App configuration
├── App.xaml.cs                   # Entry point
└── MonitorColorControl.csproj    # Project file
```

## Usage

1. **Launch the application**
2. **Adjust sliders** - Changes apply in real-time with preview
3. **Create profiles** - Name and save custom configurations
4. **Switch profiles** - Use hotkeys or dropdown menu
5. **System tray** - Minimize to tray and access profiles

## Default Hotkeys

- `Ctrl+Alt+1` - FPS Competitive profile
- `Ctrl+Alt+2` - Fortnite Color Boost profile
- `Ctrl+Alt+3` - CS2 Visibility Boost profile
- `Ctrl+Alt+4` - Movie Mode profile
- `Ctrl+Alt+5` - Night Mode profile
- `Ctrl+Alt+R` - Reset to default

## Settings Storage

Profiles are stored in: `%APPDATA%\MonitorColorControl\profiles\`

```json
{
  "name": "My Profile",
  "description": "Custom gaming profile",
  "category": "Gaming",
  "monitorSettings": {
    "Display 1": {
      "gamma": 1.2,
      "brightness": 15,
      "contrast": 120,
      "saturation": 130,
      "vibrance": 80
    }
  }
}
```

## Preset Profiles

### FPS Competitive
- Enhanced visibility for competitive gaming
- Increased contrast and saturation
- Optimized brightness

### Fortnite Color Boost
- Vibrant colors for better enemy visibility
- High saturation and vibrance
- Balanced brightness

### CS2 Visibility Boost
- Enhanced smoke/utility visibility
- High contrast and brightness
- Optimized gamma

### Movie Mode
- Cinema-like color accuracy
- Reduced vibrance and saturation
- Proper black levels

### Night Mode
- Blue light reduction
- Lower brightness
- Warm color temperature

## Technical Details

### Gamma Control
- Uses Windows `SetDeviceGammaRamp` WinAPI
- Applies directly to GPU color LUT
- Instant application without restart

### Color Effects
- Software-based color transforms
- DirectX-compatible shaders
- Per-monitor color space conversion

### Multi-Monitor Support
- Independent settings per monitor
- Option to apply to all monitors
- Automatic monitor detection

### Performance
- Minimal CPU usage (~1-2%)
- Low memory footprint (~50MB)
- Instant slider response
- Optimized for real-time preview

## System Requirements

- **OS:** Windows 10 (Build 1809+) or Windows 11
- **GPU:** Any DirectX 11+ capable GPU
- **.NET:** Runtime 8.0 or later
- **RAM:** Minimum 256MB
- **CPU:** Any modern processor

## Troubleshooting

### Settings Don't Persist
- Enable "Auto-apply on startup" in Settings
- Check if profile folder has write permissions
- Verify profile file exists in `%APPDATA%\MonitorColorControl\profiles\`

### Monitor Not Detected
- Try "Refresh Monitors" button
- Restart application
- Check Windows Display Settings
- Ensure monitor driver is up to date

### Extreme Mode Grayed Out
- Enable in Settings → Advanced
- Restart application after enabling

### High CPU Usage
- Close other applications
- Disable real-time preview
- Check for driver conflicts

## Advanced Settings

### Config File
Edit `%APPDATA%\MonitorColorControl\settings.json`:

```json
{
  "enableExtremeMode": false,
  "autoStartMinimized": true,
  "enableSystemTray": true,
  "previewUpdateDelayMs": 100,
  "defaultProfile": "FPS Competitive"
}
```

## Development

### Build Requirements
- Visual Studio 2022 (17.0+) or VS Code
- .NET 8 SDK
- Windows 10/11 development environment

### Code Style
- Follow C# naming conventions
- XML documentation on public members
- Use LINQ where appropriate
- Error handling on all I/O operations

## Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Commit with descriptive messages
4. Push to your fork
5. Create a Pull Request

## Known Limitations

- Requires administrator privileges for some monitor types
- Some older GPUs may have limited color LUT precision
- HDR monitors may need additional configuration
- Some games may override color settings in fullscreen mode

## Performance Impact

- **CPU:** <2% (idle state)
- **Memory:** ~50-80MB
- **Disk:** ~5MB (application + profiles)
- **GPU:** Negligible impact (color LUT only)

## License

MIT License - See LICENSE file for details

## Support

For issues and feature requests:
- GitHub Issues: https://github.com/kozelskysamuellll/MonitorColorControl/issues
- Documentation: Check the wiki

---

**Version:** 1.0.0  
**Last Updated:** 2026-06-13  
**Author:** Samuel Kozelsky  
**License:** MIT