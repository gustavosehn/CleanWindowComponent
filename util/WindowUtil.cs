using cwc.render;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CleanWindowComponent;

public abstract class CwcWindow : Window
{
    protected DrawingVisualHost VisualHost { get; private set; }
    protected DrawingVisual RootVisual { get; private set; }
    protected DispatcherTimer RenderTimer { get; private set; }

    protected abstract void OnCwcRender(DrawingContext dc);
    protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e) { }
    protected virtual void OnMouseMove(object sender, MouseEventArgs e) { }
    protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e) { }
    protected virtual void OnKeyDown(object sender, KeyEventArgs e) { }

    protected CwcWindow(string title = "CWC Window", double width = 800, double height = 600)
    {
        Title = title;
        Width = width;
        Height = height;
        WindowStyle = WindowStyle.None;
        Background = Brushes.Black;
        ResizeMode = ResizeMode.NoResize;

        VisualHost = new DrawingVisualHost();
        RootVisual = new DrawingVisual();
        Content = VisualHost;

        RenderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        RenderTimer.Tick += (s, e) => Render();

        PreviewMouseLeftButtonDown += OnMouseDown;
        PreviewMouseLeftButtonUp += OnMouseUp;
        PreviewMouseMove += OnMouseMove;
        PreviewKeyDown += OnKeyDown;
    }

    protected void StartRenderLoop()
    {
        RenderTimer.Start();
    }

    protected void StopRenderLoop()
    {
        RenderTimer.Stop();
    }

    private void Render()
    {
        using var dc = RootVisual.RenderOpen();
        OnCwcRender(dc);
        VisualHost.SetDrawingVisual(RootVisual);
    }
}

public class CwcWindowBuilder
{
    private string _title = "CWC Window";
    private double _width = 800;
    private double _height = 600;
    private bool _fullscreen = false;
    private Color _backgroundColor = Colors.Black;
    private Action<DrawingContext>? _renderCallback;
    private Action<MouseButtonEventArgs>? _mouseDownCallback;
    private Action<MouseEventArgs>? _mouseMoveCallback;
    private Action<MouseButtonEventArgs>? _mouseUpCallback;
    private Action<KeyEventArgs>? _keyDownCallback;

    public CwcWindowBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public CwcWindowBuilder WithSize(double width, double height)
    {
        _width = width;
        _height = height;
        return this;
    }

    public CwcWindowBuilder WithFullscreen(bool fullscreen = true)
    {
        _fullscreen = fullscreen;
        return this;
    }

    public CwcWindowBuilder WithBackgroundColor(Color color)
    {
        _backgroundColor = color;
        return this;
    }

    public CwcWindowBuilder OnRender(Action<DrawingContext> callback)
    {
        _renderCallback = callback;
        return this;
    }

    public CwcWindowBuilder OnMouseDown(Action<MouseButtonEventArgs> callback)
    {
        _mouseDownCallback = callback;
        return this;
    }

    public CwcWindowBuilder OnMouseMove(Action<MouseEventArgs> callback)
    {
        _mouseMoveCallback = callback;
        return this;
    }

    public CwcWindowBuilder OnMouseUp(Action<MouseButtonEventArgs> callback)
    {
        _mouseUpCallback = callback;
        return this;
    }

    public CwcWindowBuilder OnKeyDown(Action<KeyEventArgs> callback)
    {
        _keyDownCallback = callback;
        return this;
    }

    public Window Build()
    {
        var window = new _CwcWindowImpl(
            _title, _width, _height, _fullscreen, _backgroundColor,
            _renderCallback, _mouseDownCallback, _mouseMoveCallback, _mouseUpCallback, _keyDownCallback
        );

        return window;
    }

    private class _CwcWindowImpl : CwcWindow
    {
        private readonly Action<DrawingContext>? _onRender;
        private readonly Action<MouseButtonEventArgs>? _onMouseDown;
        private readonly Action<MouseEventArgs>? _onMouseMove;
        private readonly Action<MouseButtonEventArgs>? _onMouseUp;
        private readonly Action<KeyEventArgs>? _onKeyDown;

        public _CwcWindowImpl(
            string title, double width, double height, bool fullscreen, Color bgColor,
            Action<DrawingContext>? onRender,
            Action<MouseButtonEventArgs>? onMouseDown,
            Action<MouseEventArgs>? onMouseMove,
            Action<MouseButtonEventArgs>? onMouseUp,
            Action<KeyEventArgs>? onKeyDown
        ) : base(title, width, height)
        {
            _onRender = onRender;
            _onMouseDown = onMouseDown;
            _onMouseMove = onMouseMove;
            _onMouseUp = onMouseUp;
            _onKeyDown = onKeyDown;

            Background = new SolidColorBrush(bgColor);

            if (fullscreen)
            {
                WindowState = WindowState.Maximized;
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
            }

            StartRenderLoop();
        }

        protected override void OnCwcRender(DrawingContext dc)
        {
            _onRender?.Invoke(dc);
        }

        protected override void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _onMouseDown?.Invoke(e);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            _onMouseMove?.Invoke(e);
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _onMouseUp?.Invoke(e);
        }

        protected override void OnKeyDown(object sender, KeyEventArgs e)
        {
            _onKeyDown?.Invoke(e);
        }
    }
}

public static class WindowUtil
{
    public static CwcWindowBuilder CreateWindow()
    {
        return new CwcWindowBuilder();
    }

    public static CwcWindowBuilder CreateWindow(string title)
    {
        return new CwcWindowBuilder().WithTitle(title);
    }

    public static CwcWindowBuilder CreateWindow(string title, double width, double height)
    {
        return new CwcWindowBuilder()
            .WithTitle(title)
            .WithSize(width, height);
    }
}