using System;
using System.Windows.Media;

namespace cwc.effects;

public class Glow
{
    public Color Color { get; set; } = Colors.White;
    public double Power { get; set; } = 0;
    public double Opacity { get; set; } = 1.0;
    public double Spread { get; set; } = 20;
    public bool Enabled { get; set; } = false;

    public Glow()
    {
    }

    public Glow(Color color, double power, double spread = 20, double opacity = 1.0)
    {
        Color = color;
        Power = Math.Clamp(power, 0, 100);
        Spread = spread;
        Opacity = opacity;
        Enabled = true;
    }

    public void Update()
    {
    }

    public double GetFalloff(double distance)
    {
        if (distance < 0) distance = 0;
        if (Power <= 0) return 0;

        double normalizedDist = distance / Spread;
        if (normalizedDist >= 1) return 0;

        double falloff = (1 - normalizedDist * normalizedDist) * (Power / 100.0);
        return Math.Clamp(falloff, 0, 1);
    }

    public SolidColorBrush GetBrush(double distance)
    {
        double falloff = GetFalloff(distance);
        double alpha = 255 * falloff * Opacity;

        return new SolidColorBrush(Color.FromArgb(
            (byte)Math.Clamp(alpha, 0, 255),
            Color.R,
            Color.G,
            Color.B
        ));
    }
}
