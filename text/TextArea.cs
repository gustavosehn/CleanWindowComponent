using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Text.RegularExpressions;
using CleanWindowComponent.animation;

namespace cwc.text;

public class TextArea
{
    public enum CharacterMode
    {
        All,
        AlphaNumeric,
        Numeric,
        AlphaOnly,
        Custom
    }

    public string Content { get; set; } = "";
    public string Placeholder { get; set; } = "";
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;
    public double Width { get; set; } = 300;
    public double Height { get; set; } = 150;
    public double CornerRadius { get; set; } = 8;
    public Color TextColor { get; set; } = Colors.Black;
    public Color PlaceholderColor { get; set; } = Color.FromArgb(255, 150, 150, 150);
    public Color BackgroundColor { get; set; } = Colors.White;
    public Color BorderColor { get; set; } = Color.FromArgb(255, 200, 200, 200);
    public Color FocusedBorderColor { get; set; } = Color.FromArgb(255, 0, 122, 255);
    public double BorderWidth { get; set; } = 1;
    public double FontSize { get; set; } = 14;
    public double Padding { get; set; } = 10;
    public int MaxCharacters { get; set; } = -1;
    public CharacterMode Mode { get; set; } = CharacterMode.All;
    public string CustomPattern { get; set; } = "";
    public double LineHeight { get; set; } = 1.5;
    public bool IsFocused { get; set; } = false;
    public Origin Origin { get; set; } = Origin.Center;

    private List<string> _lines = new List<string> { "" };
    private int _cursorLineIndex = 0;
    private int _cursorCharIndex = 0;

    public TextArea() { }

    public TextArea(double width = 300, double height = 150, string placeholder = "")
    {
        Width = width;
        Height = height;
        Placeholder = placeholder;
    }

    public void AddCharacter(char c)
    {
        if (MaxCharacters > 0 && Content.Length >= MaxCharacters)
            return;

        if (!IsCharacterAllowed(c))
            return;

        if (_cursorLineIndex >= _lines.Count)
            _lines.Add("");

        string line = _lines[_cursorLineIndex];
        _lines[_cursorLineIndex] = line.Insert(_cursorCharIndex, c.ToString());
        _cursorCharIndex++;
        UpdateContent();
    }

    public void RemoveCharacter()
    {
        if (_cursorCharIndex > 0)
        {
            string line = _lines[_cursorLineIndex];
            _lines[_cursorLineIndex] = line.Remove(_cursorCharIndex - 1, 1);
            _cursorCharIndex--;
            UpdateContent();
        }
    }

    public void NewLine()
    {
        if (_cursorLineIndex >= _lines.Count)
            _lines.Add("");

        string currentLine = _lines[_cursorLineIndex];
        string afterCursor = currentLine.Substring(_cursorCharIndex);
        _lines[_cursorLineIndex] = currentLine.Substring(0, _cursorCharIndex);
        _lines.Insert(_cursorLineIndex + 1, afterCursor);
        _cursorLineIndex++;
        _cursorCharIndex = 0;
        UpdateContent();
    }

    public void SetContent(string text)
    {
        Content = text;
        _lines = new List<string>(text.Split('\n'));
        _cursorLineIndex = 0;
        _cursorCharIndex = 0;
    }

    private void UpdateContent()
    {
        Content = string.Join("\n", _lines);
    }

    private bool IsCharacterAllowed(char c)
    {
        return Mode switch
        {
            CharacterMode.AlphaNumeric => char.IsLetterOrDigit(c),
            CharacterMode.Numeric => char.IsDigit(c),
            CharacterMode.AlphaOnly => char.IsLetter(c),
            CharacterMode.Custom => !string.IsNullOrEmpty(CustomPattern) && Regex.IsMatch(c.ToString(), CustomPattern),
            _ => true
        };
    }

    public void Draw(DrawingContext dc)
    {
        var offset = OriginExtensions.GetOriginOffset(Origin, Width, Height);
        double left = X - offset.X;
        double top = Y - offset.Y;

        Color currentBorderColor = IsFocused ? FocusedBorderColor : BorderColor;
        var backgroundBrush = new SolidColorBrush(BackgroundColor);
        var borderPen = new Pen(new SolidColorBrush(currentBorderColor), BorderWidth);

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

        string displayText = string.IsNullOrEmpty(Content) ? Placeholder : Content;
        Color displayColor = string.IsNullOrEmpty(Content) ? PlaceholderColor : TextColor;

        var typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        var formattedText = new FormattedText(
            displayText,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            new SolidColorBrush(displayColor),
            VisualTreeHelper.GetDpi(new System.Windows.Controls.Canvas()).PixelsPerDip
        );

        formattedText.MaxTextWidth = Width - (Padding * 2);

        dc.DrawText(formattedText, new System.Windows.Point(left + Padding, top + Padding));
    }
}