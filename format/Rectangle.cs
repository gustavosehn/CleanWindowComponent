using CleanWindowComponent.animation;
using cwc.effects;
using cwc.format;
using cwc.util;
using System;
using System.Windows.Media;

namespace CleanWindowComponent;

public class Rectangle
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public Origin Origin { get; set; } = Origin.Center;

    public double CornerRadius { get; set; } = 0;
    public double TopLeftRadius { get; set; } = -1;
    public double TopRightRadius { get; set; } = -1;
    public double BottomLeftRadius { get; set; } = -1;
    public double BottomRightRadius { get; set; } = -1;

    public Fill Fill { get; set; } = new Fill();
    public Outline Outline { get; set; } = new Outline();
    public Shadow Shadow { get; set; } = new Shadow();
    public Glow Glow { get; set; } = new Glow();

    public Rectangle? FillParent { get; set; }

    private double _lastParentX = 0;
    private double _lastParentY = 0;

    private (double left, double top) GetDrawPosition()
    {
        var offset = OriginExtensions.GetOriginOffset(Origin, Width, Height);
        return (X - offset.X, Y - offset.Y);
    }

    public bool IsPointInside(double px, double py)
    {
        var (left, top) = GetDrawPosition();

        double tl = TopLeftRadius >= 0 ? TopLeftRadius : CornerRadius;
        double tr = TopRightRadius >= 0 ? TopRightRadius : CornerRadius;
        double bl = BottomLeftRadius >= 0 ? BottomLeftRadius : CornerRadius;
        double br = BottomRightRadius >= 0 ? BottomRightRadius : CornerRadius;

        return Collisions.IsPointInsideCustom(left, top, Width, Height, tl, tr, bl, br, px, py);
    }

    public void Update()
    {
        Fill.Update();
        Outline.Update();
        Shadow.Update();
        Glow.Update();
    }

    public void FollowParent(double parentX, double parentY)
    {
        if (FillParent is null) return;

        double parentDeltaX = parentX - _lastParentX;
        double parentDeltaY = parentY - _lastParentY;

        X += parentDeltaX;
        Y += parentDeltaY;

        _lastParentX = parentX;
        _lastParentY = parentY;
    }

    public void SetParentPosition(double parentX, double parentY)
    {
        _lastParentX = parentX;
        _lastParentY = parentY;
    }

    public Rectangle CreateChild(double x, double y, double width, double height)
    {
        var child = new Rectangle
        {
            X = x,
            Y = y,
            Width = width,
            Height = height,
            FillParent = this
        };
        return child;
    }

    public double GetAbsoluteX()
    {
        var (left, _) = GetDrawPosition();
        if (FillParent is null) return left;
        return FillParent.GetAbsoluteX() + left;
    }

    public double GetAbsoluteY()
    {
        var (_, top) = GetDrawPosition();
        if (FillParent is null) return top;
        return FillParent.GetAbsoluteY() + top;
    }

    public void ClampToParent()
    {
        if (FillParent is null) return;

        Width = Math.Min(Width, FillParent.Width);
        Height = Math.Min(Height, FillParent.Height);

        X = Math.Max(0, Math.Min(X, FillParent.Width - Width));
        Y = Math.Max(0, Math.Min(Y, FillParent.Height - Height));
    }

    private StreamGeometry CreateGeometry()
    {
        var (left, top) = GetDrawPosition();

        double maxRadius = Math.Min(Width, Height) / 2.0;

        double rTL = Math.Min(TopLeftRadius >= 0 ? TopLeftRadius : CornerRadius, maxRadius);
        double rTR = Math.Min(TopRightRadius >= 0 ? TopRightRadius : CornerRadius, maxRadius);
        double rBL = Math.Min(BottomLeftRadius >= 0 ? BottomLeftRadius : CornerRadius, maxRadius);
        double rBR = Math.Min(BottomRightRadius >= 0 ? BottomRightRadius : CornerRadius, maxRadius);

        StreamGeometry geometry = new StreamGeometry();
        using (StreamGeometryContext context = geometry.Open())
        {
            context.BeginFigure(new System.Windows.Point(left + rTL, top), true, true);

            context.LineTo(new System.Windows.Point(left + Width - rTR, top), true, false);
            if (rTR > 0) context.ArcTo(new System.Windows.Point(left + Width, top + rTR), new System.Windows.Size(rTR, rTR), 0, false, SweepDirection.Clockwise, true, false);

            context.LineTo(new System.Windows.Point(left + Width, top + Height - rBR), true, false);
            if (rBR > 0) context.ArcTo(new System.Windows.Point(left + Width - rBR, top + Height), new System.Windows.Size(rBR, rBR), 0, false, SweepDirection.Clockwise, true, false);

            context.LineTo(new System.Windows.Point(left + rBL, top + Height), true, false);
            if (rBL > 0) context.ArcTo(new System.Windows.Point(left, top + Height - rBL), new System.Windows.Size(rBL, rBL), 0, false, SweepDirection.Clockwise, true, false);

            context.LineTo(new System.Windows.Point(left, top + rTL), true, false);
            if (rTL > 0) context.ArcTo(new System.Windows.Point(left + rTL, top), new System.Windows.Size(rTL, rTL), 0, false, SweepDirection.Clockwise, true, false);
        }

        geometry.Freeze();
        return geometry;
    }

    public void Draw(DrawingContext dc)
    {
        StreamGeometry geometry = CreateGeometry();

        if (Fill.Enabled)
        {
            dc.DrawGeometry(Fill.GetBrush(), null, geometry);
        }

        if (Outline.Enabled)
        {
            var outlinePen = new System.Windows.Media.Pen(Outline.GetBrush(), Outline.Width);
            dc.DrawGeometry(null, outlinePen, geometry);
        }
    }
}