using System;
using System.Windows;

namespace CleanWindowComponent.animation;

public enum Origin
{
    TopLeft,
    Top,
    TopRight,
    Right,
    BottomRight,
    Bottom,
    BottomLeft,
    Left,
    Center
}

public static class OriginExtensions
{
    public static Point GetOriginOffset(Origin origin, double width, double height)
    {
        return origin switch
        {
            Origin.TopLeft => new Point(0, 0),
            Origin.Top => new Point(width / 2, 0),
            Origin.TopRight => new Point(width, 0),
            Origin.Right => new Point(width, height / 2),
            Origin.BottomRight => new Point(width, height),
            Origin.Bottom => new Point(width / 2, height),
            Origin.BottomLeft => new Point(0, height),
            Origin.Left => new Point(0, height / 2),
            Origin.Center => new Point(width / 2, height / 2),
            _ => new Point(width / 2, height / 2)
        };
    }

    public static (double x, double y) ApplyOrigin(double x, double y, Origin origin, double width, double height)
    {
        var offset = GetOriginOffset(origin, width, height);
        return (x - offset.X, y - offset.Y);
    }

    public static (double x, double y) ReverseOrigin(double x, double y, Origin origin, double width, double height)
    {
        var offset = GetOriginOffset(origin, width, height);
        return (x + offset.X, y + offset.Y);
    }
}
