using System;
using System.IO;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace cwc.util;

public class Svg
{
    private string _svgContent;
    private BitmapSource? _cachedImage;

    public Svg()
    {
        _svgContent = "";
        _cachedImage = null;
    }

    public Svg(string svgContent)
    {
        _svgContent = svgContent;
        _cachedImage = null;
    }

    public void LoadFromXml(string xmlContent)
    {
        try
        {
            XDocument doc = XDocument.Parse(xmlContent);
            _svgContent = xmlContent;
            _cachedImage = null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Invalid SVG XML", ex);
        }
    }

    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"SVG file not found: {filePath}");

        string content = File.ReadAllText(filePath);
        LoadFromXml(content);
    }

    public BitmapSource? ConvertToImage(int width = 24, int height = 24)
    {
        if (_cachedImage != null)
            return _cachedImage;

        if (string.IsNullOrEmpty(_svgContent))
            return null;

        try
        {
            WpfDrawingSettings settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true
            };

            FileSvgReader reader = new FileSvgReader(settings);
            string tempSvgPath = Path.Combine(Path.GetTempPath(), $"temp_{Guid.NewGuid()}.svg");

            File.WriteAllText(tempSvgPath, _svgContent);

            System.Windows.Media.DrawingGroup drawing = reader.Read(tempSvgPath);
            File.Delete(tempSvgPath);

            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, 96, 96, 
                System.Windows.Media.PixelFormats.Pbgra32);
            
            System.Windows.Media.DrawingVisual visual = new System.Windows.Media.DrawingVisual();
            using (System.Windows.Media.DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawDrawing(drawing);
            }

            bitmap.Render(visual);
            bitmap.Freeze();

            _cachedImage = bitmap;
            return _cachedImage;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to convert SVG to image", ex);
        }
    }

    public BitmapSource? GetImage()
    {
        return ConvertToImage();
    }
}
