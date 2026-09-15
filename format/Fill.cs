using System;
using System.Windows.Media;

namespace cwc.format;

public class Fill
{
    public enum FillType
    {
        Solid,
        Glass
    }

    public Color Color { get; set; } = Colors.White;
    public double Opacity { get; set; } = 1.0;
    public FillType Type { get; set; } = FillType.Solid;
    public bool Enabled { get; set; } = false;

    public Fill()
    {
    }

    public Fill(Color color, double opacity = 1.0, FillType type = FillType.Solid)
    {
        Color = color;
        Opacity = Math.Clamp(opacity, 0, 1);
        Type = type;
        Enabled = true;
    }

    public void Update()
    {
    }

    public SolidColorBrush GetBrush()
    {
        byte alpha = (byte)(255 * Opacity);
        
        if (Type == FillType.Glass)
        {
            return new SolidColorBrush(Color.FromArgb(
                (byte)(alpha * 0.7),
                (byte)(Color.R * 1.1),
                (byte)(Color.G * 1.1),
                (byte)(Color.B * 1.1)
            ));
        }

        return new SolidColorBrush(Color.FromArgb(
            alpha,
            Color.R,
            Color.G,
            Color.B
        ));
    }
}
