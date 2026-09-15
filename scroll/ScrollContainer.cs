using CleanWindowComponent;
using System;
using System.Collections.Generic;

namespace cwc.scroll;

public class ScrollContainer
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    
    public double ScrollOffsetY { get; set; } = 0;
    public double ScrollOffsetX { get; set; } = 0;
    
    public double ContentHeight { get; set; }
    public double ContentWidth { get; set; }
    
    public ScrollBar VerticalScrollBar { get; set; }
    public ScrollBar HorizontalScrollBar { get; set; }
    
    public bool EnableVerticalScroll { get; set; } = true;
    public bool EnableHorizontalScroll { get; set; } = false;

    public ScrollContainer(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        ContentHeight = height;
        ContentWidth = width;
        
        VerticalScrollBar = new ScrollBar(ScrollBar.Orientation.Vertical);
        HorizontalScrollBar = new ScrollBar(ScrollBar.Orientation.Horizontal);
    }

    public void Update(List<Rectangle> items)
    {
        double maxY = 0;
        double maxX = 0;

        for (int i = 1; i < items.Count; i++)
        {
            var item = items[i];
            if (item.Y + item.Height > maxY)
                maxY = item.Y + item.Height;
            if (item.X + item.Width > maxX)
                maxX = item.X + item.Width;
        }

        ContentHeight = maxY;
        ContentWidth = maxX;

        if (EnableVerticalScroll)
        {
            double scrollableHeight = Math.Max(0, ContentHeight - Height);
            ScrollOffsetY = Math.Clamp(ScrollOffsetY, 0, scrollableHeight);
            
            VerticalScrollBar.X = X + Width - 8;
            VerticalScrollBar.Y = Y;
            VerticalScrollBar.Height = Height;
            
            VerticalScrollBar.Update(ScrollOffsetY, Height, ContentHeight);
        }

        if (EnableHorizontalScroll)
        {
            double scrollableWidth = Math.Max(0, ContentWidth - Width);
            ScrollOffsetX = Math.Clamp(ScrollOffsetX, 0, scrollableWidth);
            
            HorizontalScrollBar.X = X;
            HorizontalScrollBar.Y = Y + Height - 8;
            HorizontalScrollBar.Width = Width;
            
            HorizontalScrollBar.Update(ScrollOffsetX, Width, ContentWidth);
        }
    }

    public void Scroll(double deltaY)
    {
        if (!EnableVerticalScroll) return;
        
        double scrollableHeight = Math.Max(0, ContentHeight - Height);
        ScrollOffsetY = Math.Clamp(ScrollOffsetY + deltaY, 0, scrollableHeight);
    }

    public void SetScrollFromThumbPosition(double thumbPos, bool isVertical)
    {
        if (isVertical && EnableVerticalScroll)
        {
            double newOffset = VerticalScrollBar.GetScrollOffsetFromThumbPosition(
                thumbPos, Height, ContentHeight);
            ScrollOffsetY = newOffset;
        }
    }

    public bool IsPointInViewport(double px, double py)
    {
        return px >= X && px <= X + Width && py >= Y && py <= Y + Height;
    }

    public bool IsRectangleInViewport(Rectangle rect)
    {
        double rectBottom = rect.Y + rect.Height - ScrollOffsetY;
        double rectTop = rect.Y - ScrollOffsetY;

        return !(rectBottom < Y || rectTop > Y + Height);
    }
}