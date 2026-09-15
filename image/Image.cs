using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CleanWindowComponent;

public class Image
{
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Width { get; set; } = 100;
    public double Height { get; set; } = 100;
    public double Scale { get; set; } = 1.0;
    public double Opacity { get; set; } = 1.0;
    public double Rotation { get; set; } = 0;

    private BitmapSource? _imageSource;
    private string? _loadedPath;
    private bool _isLoading = false;

    public Image() { }

    public Image(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Image file not found: {filePath}");

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            _imageSource = bitmap;
            _loadedPath = filePath;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load image from file: {filePath}", ex);
        }
    }

    public async Task LoadFromUrlAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be null or empty");

        _isLoading = true;

        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    _imageSource = bitmap;
                    _loadedPath = url;
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load image from URL: {url}", ex);
        }
        finally
        {
            _isLoading = false;
        }
    }

    public void LoadFromUrl(string url)
    {
        Task.Run(() => LoadFromUrlAsync(url)).Wait();
    }

    public bool IsLoaded() => _imageSource != null && !_isLoading;

    public void Draw(DrawingContext dc)
    {
        if (_imageSource == null)
            return;

        double scaledWidth = Width * Scale;
        double scaledHeight = Height * Scale;

        var rect = new System.Windows.Rect(X - scaledWidth / 2, Y - scaledHeight / 2, scaledWidth, scaledHeight);

        if (Rotation != 0)
        {
            dc.PushTransform(new RotateTransform(Rotation, X, Y));
            dc.DrawImage(_imageSource, rect);
            dc.Pop();
        }
        else
        {
            if (Opacity < 1.0)
            {
                dc.PushOpacity(Opacity);
                dc.DrawImage(_imageSource, rect);
                dc.Pop();
            }
            else
            {
                dc.DrawImage(_imageSource, rect);
            }
        }
    }

    public void SetSize(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void SetPosition(double x, double y)
    {
        X = x;
        Y = y;
    }

    public void SetScale(double scale)
    {
        Scale = Math.Max(0.1, scale);
    }

    public void SetOpacity(double opacity)
    {
        Opacity = Math.Clamp(opacity, 0, 1);
    }

    public void SetRotation(double rotation)
    {
        Rotation = rotation % 360;
    }

    public double GetActualWidth() => Width * Scale;
    public double GetActualHeight() => Height * Scale;
}