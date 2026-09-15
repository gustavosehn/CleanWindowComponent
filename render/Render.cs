using CleanWindowComponent;
using System.Windows.Media;

namespace cwc.render;

public static class Render
{
    public static void DrawRectangle(DrawingContext dc, Rectangle rect)
    {
        if (rect.Fill.Enabled)
        {
            dc.DrawRoundedRectangle(
                rect.Fill.GetBrush(),
                null,
                new System.Windows.Rect(rect.X, rect.Y, rect.Width, rect.Height),
                rect.CornerRadius,
                rect.CornerRadius
            );
        }

        if (rect.Outline.Enabled)
        {
            dc.DrawRoundedRectangle(
                null,
                new Pen(rect.Outline.GetBrush(), rect.Outline.Width),
                new System.Windows.Rect(rect.X, rect.Y, rect.Width, rect.Height),
                rect.CornerRadius,
                rect.CornerRadius
            );
        }
    }

    public static void DrawRectangleAt(DrawingContext dc, Rectangle rect, double x, double y)
    {
        if (rect.Fill.Enabled)
        {
            dc.DrawRoundedRectangle(
                rect.Fill.GetBrush(),
                null,
                new System.Windows.Rect(x, y, rect.Width, rect.Height),
                rect.CornerRadius,
                rect.CornerRadius
            );
        }

        if (rect.Outline.Enabled)
        {
            dc.DrawRoundedRectangle(
                null,
                new Pen(rect.Outline.GetBrush(), rect.Outline.Width),
                new System.Windows.Rect(x, y, rect.Width, rect.Height),
                rect.CornerRadius,
                rect.CornerRadius
            );
        }
    }
}