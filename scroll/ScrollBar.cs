using System;
using System.Windows.Media;

namespace cwc.scroll;

public class ScrollBar
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public Orientation Direction { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 8;
    public double Height { get; set; }

    public double ThumbPosition { get; set; }
    public double ThumbSize { get; set; }

    public Color Color { get; set; } = Colors.Gray;
    public double Opacity { get; set; } = 0.6;
    public double HoverOpacity { get; set; } = 0.9;

    public bool IsHovered { get; set; } = false;

    public ScrollBar(Orientation orientation)
    {
        Direction = orientation;
    }

    public void Update(double offset, double viewportSize, double contentSize)
    {
        if (contentSize <= 0 || viewportSize <= 0) return;

        if (contentSize <= viewportSize)
        {
            ThumbSize = viewportSize;
            ThumbPosition = 0;
            return;
        }

        double trackSize = Direction == Orientation.Vertical ? Height : Width;
        ThumbSize = (viewportSize / contentSize) * trackSize;
        ThumbSize = Math.Max(20, ThumbSize);

        double scrollableSize = contentSize - viewportSize;
        double trackScrollable = trackSize - ThumbSize;
        
        if (scrollableSize <= 0)
        {
            ThumbPosition = 0;
            return;
        }

        double scrollPercent = offset / scrollableSize;
        ThumbPosition = Math.Clamp(scrollPercent * trackScrollable, 0, trackScrollable);
    }

    public bool IsThumbHovered(double px, double py)
    {
        if (Direction == Orientation.Vertical)
        {
            return px >= X && px <= X + Width && 
                   py >= Y + ThumbPosition && py <= Y + ThumbPosition + ThumbSize;
        }
        else
        {
            return px >= X + ThumbPosition && px <= X + ThumbPosition + ThumbSize && 
                   py >= Y && py <= Y + Height;
        }
    }

    public double GetScrollOffsetFromThumbPosition(double thumbPos, double viewportSize, double contentSize)
    {
        if (contentSize <= viewportSize) return 0;

        double scrollableSize = contentSize - viewportSize;
        double trackSize = Direction == Orientation.Vertical ? Height : Width;
        double trackScrollable = trackSize - ThumbSize;

        if (trackScrollable <= 0) return 0;

        double scrollPercent = thumbPos / trackScrollable;
        return scrollPercent * scrollableSize;
    }

    public double GetThumbPositionFromClickPoint(double clickPos)
    {
        double trackSize = Direction == Orientation.Vertical ? Height : Width;
        double newPosition = clickPos - (ThumbSize / 2);
        return Math.Clamp(newPosition, 0, trackSize - ThumbSize);
    }

    public void Draw(DrawingContext dc)
    {
        double opacity = IsHovered ? HoverOpacity : Opacity;
        var brush = new SolidColorBrush(Color);
        brush.Opacity = opacity;

        if (Direction == Orientation.Vertical)
        {
            dc.DrawRectangle(
                brush,
                null,
                new System.Windows.Rect(X, Y + ThumbPosition, Width, ThumbSize)
            );
        }
        else
        {
            dc.DrawRectangle(
                brush,
                null,
                new System.Windows.Rect(X + ThumbPosition, Y, ThumbSize, Height)
            );
        }
    }
}