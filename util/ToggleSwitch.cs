using System;
using System.Windows;
using System.Windows.Media;

namespace cwc.util;

public class ToggleSwitch
{
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Width { get; set; } = 50;
    public double Height { get; set; } = 28;
    public bool IsEnabled { get; set; } = false;
    public Color EnabledColor { get; set; } = Color.FromArgb(255, 52, 168, 83);
    public Color DisabledColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
    public Color ThumbColor { get; set; } = Colors.White;
    public bool HasAnimation { get; set; } = true;
    public int AnimationFrames { get; set; } = 10;
    public bool IsHovered { get; set; } = false;

    private double _thumbPosition = 0;
    private double _targetThumbPosition = 0;
    private Color _currentBackgroundColor;
    private int _animationCounter = 0;

    public ToggleSwitch() 
    {
        _currentBackgroundColor = DisabledColor;
        _thumbPosition = CalculateThumbPosition();
    }

    public ToggleSwitch(double width = 50, double height = 28, bool initialState = false)
    {
        Width = width;
        Height = height;
        IsEnabled = initialState;
        _thumbPosition = CalculateThumbPosition();
        _targetThumbPosition = _thumbPosition;
        _currentBackgroundColor = IsEnabled ? EnabledColor : DisabledColor;
    }

    public void Toggle()
    {
        IsEnabled = !IsEnabled;
        UpdateAnimation();
    }

    public void SetState(bool enabled)
    {
        IsEnabled = enabled;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        _targetThumbPosition = CalculateThumbPosition();

        if (HasAnimation && AnimationFrames > 0)
            _animationCounter = 0;
        else
        {
            _thumbPosition = _targetThumbPosition;
            _currentBackgroundColor = IsEnabled ? EnabledColor : DisabledColor;
        }
    }

    public void UpdateFrame()
    {
        if (!HasAnimation || AnimationFrames <= 0)
            return;

        if (_animationCounter < AnimationFrames)
        {
            _animationCounter++;
            double progress = (double)_animationCounter / AnimationFrames;
            _thumbPosition = Lerp(_thumbPosition, _targetThumbPosition, progress);
            _currentBackgroundColor = LerpColor(
                IsEnabled ? DisabledColor : EnabledColor,
                IsEnabled ? EnabledColor : DisabledColor,
                progress
            );
        }
        else
        {
            _thumbPosition = _targetThumbPosition;
            _currentBackgroundColor = IsEnabled ? EnabledColor : DisabledColor;
        }
    }

    public bool IsPointInside(double px, double py)
    {
        return px >= X && px <= X + Width && py >= Y && py <= Y + Height;
    }

    public void Draw(DrawingContext dc)
    {
        UpdateFrame();

        var backgroundBrush = new SolidColorBrush(_currentBackgroundColor);
        double cornerRadius = Height / 2;

        var rect = new Rect(X, Y, Width, Height);
        dc.DrawRoundedRectangle(backgroundBrush, null, rect, cornerRadius, cornerRadius);

        double thumbRadius = Height / 2 - 2;
        double thumbX = X + _thumbPosition;
        double thumbY = Y + Height / 2;

        var thumbBrush = new SolidColorBrush(ThumbColor);
        dc.DrawEllipse(thumbBrush, null, new Point(thumbX, thumbY), thumbRadius, thumbRadius);
    }

    private double CalculateThumbPosition()
    {
        double thumbRadius = Height / 2 - 2;
        double minPos = thumbRadius + 2;
        double maxPos = Width - thumbRadius - 2;
        return IsEnabled ? maxPos : minPos;
    }

    private double Lerp(double from, double to, double t) => from + (to - from) * t;

    private Color LerpColor(Color from, Color to, double t)
    {
        byte lerpByte(byte f, byte tt) => (byte)(f + (tt - f) * t);
        return Color.FromArgb(
            lerpByte(from.A, to.A),
            lerpByte(from.R, to.R),
            lerpByte(from.G, to.G),
            lerpByte(from.B, to.B)
        );
    }
}