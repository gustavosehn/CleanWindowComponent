# Clean Window Component

An open-source drawing library for building custom UI components in C#.

## Overview

Clean Window Component (CWC) is a lightweight, high-performance drawing library built on top of WPF's DrawingVisual API. It enables developers to create custom, visually appealing UI components with pixel-perfect control over rendering.

## Features

- Custom component drawing with DrawingVisual
- Smooth animations and transitions
- Event handling for mouse and keyboard input
- Built-in UI components (buttons, sliders, toggles, text inputs)
- Full control over rendering pipeline
- Minimal overhead and high performance
- Cross-platform compatibility with Windows

## Getting Started

### Installation

Add the CWC library to your project by referencing the compiled DLL or adding it as a project reference.

### Basic Usage

```csharp
using CleanWindowComponent;
using System.Windows.Media;

var window = WindowUtil.CreateWindow("My App", 800, 600)
    .WithBackgroundColor(Colors.Black)
    .OnRender(dc =>
    {
        // Draw your content here
        dc.DrawRectangle(
            new SolidColorBrush(Colors.Blue),
            null,
            new Rect(50, 50, 200, 100)
        );
    })
    .Build();

window.Show();
```

## Components

### CwcWindow

Base class for creating custom windows with rendering support.

```csharp
public class MyWindow : CwcWindow
{
    protected override void OnCwcRender(DrawingContext dc)
    {
        // Implement your rendering logic
    }
}
```

### CwcWindowBuilder

Fluent API for creating and configuring windows.

```csharp
var window = new CwcWindowBuilder()
    .WithTitle("My Application")
    .WithSize(1024, 768)
    .WithBackgroundColor(Color.FromArgb(255, 30, 30, 40))
    .OnRender(dc => { /* rendering */ })
    .OnMouseDown(e => { /* mouse handling */ })
    .Build();
```

### Built-in Components

- Rectangle: Drawable rectangles with customizable appearance
- ToggleSwitch: Interactive toggle switches
- Slider: Draggable slider components
- Text: Text rendering with custom fonts and colors
- TextBox: Single-line text input
- TextArea: Multi-line text input

## Animation System

CWC includes a keyframe-based animation system for smooth transitions.

```csharp
var animation = new KeyframeAnimation("ButtonClick", 12);
animation.SetKeyframe("Fill.Color", 0, Color.FromArgb(255, 30, 144, 255));
animation.SetKeyframe("Fill.Color", 6, Color.FromArgb(255, 100, 180, 255));
animation.SetKeyframe("Fill.Color", 12, Color.FromArgb(255, 30, 144, 255));

animator.RegisterAnimation(animation);
animator.PlayAnimation("ButtonClick", target);
```

## Event Handling

Handle user interactions through mouse and keyboard events.

```csharp
protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
{
    var position = e.GetPosition(null);
    if (myComponent.IsPointInside(position.X, position.Y))
    {
        myComponent.HandleClick();
    }
}
```

## Architecture

CWC follows a render-loop pattern where components are updated and drawn every frame. The rendering system is optimized for performance with minimal garbage collection overhead.

## Dependencies

- .NET 8.0 or later
- Windows-specific (requires Windows 7 or later)
- WPF (Windows Presentation Foundation)
- SharpVectors 1.8.6 (for SVG support)

## Building from Source

```bash
dotnet build -c Release
```

The compiled library will be available in the `bin/Release/net8.0-windows` directory.

## License

This project is open source and available under the MIT License.

## Contributing

Contributions are welcome. Please feel free to submit pull requests or open issues for bugs and feature requests.

## Support

For questions and support, please open an issue on the GitHub repository.
