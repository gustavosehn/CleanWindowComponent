using CleanWindowComponent.animation;
using System;
using System.Windows;
using System.Windows.Media;

namespace cwc.text;

public class TextBox
{
    public string Content { get; set; } = "";
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Width { get; set; } = 200;
    public double Height { get; set; } = 40;
    public double CornerRadius { get; set; } = 8;
    public Color TextColor { get; set; } = Colors.Black;
    public Color BackgroundColor { get; set; } = Colors.White;
    public Color BorderColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
    public double BorderWidth { get; set; } = 1;
    public double FontSize { get; set; } = 14;
    public double Padding { get; set; } = 10;
    public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;
    public Origin Origin { get; set; } = Origin.Center;

    public TextBox() { }

    public TextBox(string content, double width = 200, double height = 40)
    {
        Content = content;
        Width = width;
        Height = height;
    }

public void Draw(DrawingContext dc)
{
    var offset = OriginExtensions.GetOriginOffset(Origin, Width, Height);
    double left = X - offset.X;
    double top = Y - offset.Y;

    var backgroundBrush = new SolidColorBrush(BackgroundColor);
    var borderPen = new Pen(new SolidColorBrush(BorderColor), BorderWidth);

    var geometry = new StreamGeometry();
    using (var context = geometry.Open())
    {
        double radius = Math.Min(CornerRadius, Math.Min(Width, Height) / 2);

        context.BeginFigure(new System.Windows.Point(left + radius, top), true, true);
        context.ArcTo(new System.Windows.Point(left + Width - radius, top), new System.Windows.Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
        context.ArcTo(new System.Windows.Point(left + Width, top + radius), new System.Windows.Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
        context.ArcTo(new System.Windows.Point(left + Width - radius, top + Height), new System.Windows.Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
        context.ArcTo(new System.Windows.Point(left + radius, top + Height), new System.Windows.Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
        context.ArcTo(new System.Windows.Point(left, top + radius), new System.Windows.Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
        context.ArcTo(new System.Windows.Point(left, top + Height - radius), new System.Windows.Size(radius, radius), 0, false, SweepDirection.Clockwise, true, false);
    }

    dc.DrawGeometry(backgroundBrush, borderPen, geometry);

    var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
    var formattedText = new FormattedText(
        Content,
        System.Globalization.CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        typeface,
        FontSize,
        new SolidColorBrush(TextColor),
        VisualTreeHelper.GetDpi(new System.Windows.Controls.Canvas()).PixelsPerDip
    );

    double textX = left + Padding;
    double textY = top + (Height - formattedText.Height) / 2;

    if (TextAlignment == TextAlignment.Center)
        textX = left + (Width - formattedText.Width) / 2;
    else if (TextAlignment == TextAlignment.Right)
        textX = left + Width - Padding - formattedText.Width;

    dc.DrawText(formattedText, new System.Windows.Point(textX, textY));
}
}