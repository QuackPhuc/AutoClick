# AutoClick Tool

[![English](https://img.shields.io/badge/Language-English-blue)](README.md)
[![Tiếng Việt](https://img.shields.io/badge/Language-Tiếng%20Việt-green)](readme/README.vi.md)

A multi-functional mouse automation tool for Windows with an intuitive interface and visual script management.

## Key Features

- **Versatile Mouse Operations**:
  - Left/right mouse clicks
  - Left/right mouse drag operations
  - Mouse scrolling
- **Visual Interface**:
  - Easy point-and-click position selection
  - Visual representation of created actions
  - User-friendly control panel
- **Time Customization**:
  - Configure wait times between actions
  - Customize drag/scroll speed
- **Script Management**:
  - Save and load scripts (.acs format)
  - Easy script editing
- **System Optimization**:
  - Low resource usage
  - Stable background operation

## Usage Guide

### Creating a New Script

1. Launch the application
2. Use the control panel buttons to add actions:
   - **Left Click**: Add left click action
   - **Right Click**: Add right click action
   - **Left Drag**: Add left drag action
   - **Right Drag**: Add right drag action
   - **Mouse Scroll**: Add scroll action

3. When adding an action:
   - For click operations: Click on the desired position
   - For drag/scroll operations: Click on the start point, then drag to the end point
   - Enter wait time (in milliseconds) after the action completes
   - Click "Confirm" to add the action to the script

### Script Management

- **Delete Action**: Delete selected action from script
- **Start/Stop** (F9): Start or stop script execution
- **Save Script**: Save current script to file (.acs format)
- **Load Script**: Load previously saved script
- **Settings**: Adjust application settings
- **About**: View application information

### Advanced Settings

- **Drag/Scroll Time Ratio**: Adjust time ratio between action execution and waiting
- **Hotkey Configuration**: Customize hotkeys (default is F9)
- **Overlay Opacity**: Adjust transparency of the overlay display

## System Requirements

- Operating System: Windows 10/11
- Framework: .NET 6.0 Runtime or higher
- RAM: 2GB minimum (4GB recommended)
- Storage: Approximately 50MB disk space

## Installation

### Using Portable File

1. Download the AutoClick.exe file from the [Releases](https://github.com/QuackPhuc/AutoClick/releases) section
2. Run the executable - no installation required
3. The application will work on any Windows machine without needing to install .NET

### Running from Source

1. Ensure you have .NET 6.0 SDK or higher installed
2. Clone this repository
3. Run `dotnet build -c Release` in the src directory
4. Run the exe file from the bin/Release/net6.0-windows directory

## License

This project is distributed under the MIT License. See the LICENSE file for more information.

## Other Languages

This README is also available in:
- [Vietnamese](README.vi.md)