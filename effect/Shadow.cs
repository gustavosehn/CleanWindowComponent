using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace cwc.effects;

public class Shadow
{
    public class ShadowLayer
    {
        public double Blur { get; set; }
        public double Offset { get; set; }
        public double Opacity { get; set; }
    }

    public Color Color { get; set; } = Colors.Black;
    public double Power { get; set; } = 0;
    public double OffsetX { get; set; } = 0;
    public double OffsetY { get; set; } = 0;
    public bool Enabled { get; set; } = false;

    public List<ShadowLayer> Layers { get; set; } = new List<ShadowLayer>
    {
        new ShadowLayer { Blur = 6, Offset = 4, Opacity = 0.95 },
        new ShadowLayer { Blur = 32, Offset = 10, Opacity = 0.90 },
        new ShadowLayer { Blur = 80, Offset = 18, Opacity = 0.75 }
    };

    public Shadow()
    {
    }

    public Shadow(Color color, double power, double offsetX = 0, double offsetY = 0)
    {
        Color = color;
        Power = Math.Clamp(power, 0, 100);
        OffsetX = offsetX;
        OffsetY = offsetY;
        Enabled = true;
    }

    public void Update()
    {
    }

    public double GetDecay(double distance)
    {
        if (distance < 0) distance = 0;
        if (Power <= 0) return 0;

        double decay = Math.Exp(-distance * distance / (2 * 100));
        return decay * (Power / 100.0);
    }

    public SolidColorBrush GetBrush(ShadowLayer layer)
    {
        double alpha = 255 * layer.Opacity * (Power / 100.0);

        return new SolidColorBrush(Color.FromArgb(
            (byte)Math.Clamp(alpha, 0, 255),
            Color.R,
            Color.G,
            Color.B
        ));
    }
}