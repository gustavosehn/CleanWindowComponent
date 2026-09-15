using System;
using System.Windows;
using System.Windows.Media;

namespace cwc.text;

public class Text
{
    public string Content { get; set; } = "";
    public Color Color { get; set; } = Colors.Black;
    public double FontSize { get; set; } = 14;
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double MaxWidth { get; set; } = double.MaxValue;
    public double LineHeight { get; set; } = 1.2;
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;

    private double _measuredWidth = 0;
    private double _measuredHeight = 0;

    public Text() { }

    public Text(string content, double fontSize = 14, Color? color = null)
    {
        Content = content;
        FontSize = fontSize;
        Color = color ?? Colors.Black;
        MeasureText();
    }

    public void MeasureText()
    {
        if (string.IsNullOrEmpty(Content))
        {
            _measuredWidth = 0;
            _measuredHeight = 0;
            return;
        }

        var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        var formattedText = new FormattedText(
            Content,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            new SolidColorBrush(Color),
            VisualTreeHelper.GetDpi(new System.Windows.Controls.Canvas()).PixelsPerDip
        );

        formattedText.MaxTextWidth = MaxWidth;
        _measuredWidth = formattedText.Width;
        _measuredHeight = formattedText.Height * LineHeight;
    }

    public double GetWidth() => _measuredWidth;
    public double GetHeight() => _measuredHeight;

public void Draw(DrawingContext dc)
{
    if (string.IsNullOrEmpty(Content)) return;

    var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
    var formattedText = new FormattedText(
        Content,
        System.Globalization.CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        typeface,
        FontSize,
        new SolidColorBrush(Color),
        VisualTreeHelper.GetDpi(new System.Windows.Controls.Canvas()).PixelsPerDip
    );

    formattedText.MaxTextWidth = MaxWidth;

    double drawX = X - formattedText.Width / 2;
    double drawY = Y - formattedText.Height / 2;

    dc.DrawText(formattedText, new System.Windows.Point(drawX, drawY));
}
}

public enum TextAlignment
{
    Left,
    Center,
    Right
}