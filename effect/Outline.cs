using System.Windows.Media;

namespace cwc.effects;

public class Outline
{
    public Color Color { get; set; } = Colors.White;
    public double Width { get; set; } = 0;
    public double Opacity { get; set; } = 1.0;
    public bool Enabled { get; set; } = false;

    public Outline()
    {
    }

    public Outline(Color color, double width, double opacity = 1.0)
    {
        Color = color;
        Width = width;
        Opacity = opacity;
        Enabled = true;
    }

    public void Update()
    {
    }

    public SolidColorBrush GetBrush()
    {
        return new SolidColorBrush(Color.FromArgb(
            (byte)(Color.A * Opacity),
            Color.R,
            Color.G,
            Color.B
        ));
    }
}
