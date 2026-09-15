using System;
using System.Windows.Media;

namespace cwc.util;

public class Slider
{
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Width { get; set; } = 200;
    public double Height { get; set; } = 4;
    
    public double Value { get; set; } = 50;
    public double MinValue { get; set; } = 0;
    public double MaxValue { get; set; } = 100;
    
    public Color TrackColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
    public Color FillColor { get; set; } = Color.FromArgb(255, 0, 122, 255);
    public Color ThumbColor { get; set; } = Colors.White;
    public Color ThumbBorderColor { get; set; } = Colors.White;
    
    public double ThumbRadius { get; set; } = 10;
    public bool IsHovered { get; set; } = false;
    public bool IsDragging { get; set; } = false;
    
    private double _animatedThumbRadius;

    public Slider()
    {
        _animatedThumbRadius = ThumbRadius;
    }

    public bool IsPointInside(double px, double py)
    {
        double thumbX = X + GetThumbPositionX();
        double thumbY = Y + Height / 2;
        double distance = Math.Sqrt((px - thumbX) * (px - thumbX) + (py - thumbY) * (py - thumbY));
        return distance <= ThumbRadius + 5;
    }

    public void SetValueFromPoint(double px)
    {
        double relativeX = px - X;
        Value = MinValue + (relativeX / Width) * (MaxValue - MinValue);
        Value = Math.Clamp(Value, MinValue, MaxValue);
    }

    public void Draw(DrawingContext dc)
    {
        _animatedThumbRadius = IsHovered || IsDragging ? ThumbRadius * 1.3 : ThumbRadius;

        var trackPen = new Pen(new SolidColorBrush(TrackColor), Height);
        dc.DrawLine(trackPen, new System.Windows.Point(X, Y + Height / 2), 
                    new System.Windows.Point(X + Width, Y + Height / 2));

        double fillWidth = GetThumbPositionX();
        var fillPen = new Pen(new SolidColorBrush(FillColor), Height);
        dc.DrawLine(fillPen, new System.Windows.Point(X, Y + Height / 2),
                    new System.Windows.Point(X + fillWidth, Y + Height / 2));

        double thumbX = X + GetThumbPositionX();
        double thumbY = Y + Height / 2;
        
        var thumbBrush = new SolidColorBrush(ThumbColor);
        dc.DrawEllipse(thumbBrush, null, new System.Windows.Point(thumbX, thumbY), 
                       _animatedThumbRadius, _animatedThumbRadius);
    }

    private double GetThumbPositionX()
    {
        double range = MaxValue - MinValue;
        return (Value - MinValue) / range * Width;
    }
}