using System;
using System.Windows;
using System.Windows.Media;

namespace cwc.render;

public class DrawingVisualHost : FrameworkElement
{
    private DrawingVisual _drawingVisual;
    private VisualCollection _children;

    public DrawingVisualHost()
    {
        _children = new VisualCollection(this);
        _drawingVisual = new DrawingVisual();
        _children.Add(_drawingVisual);
    }

    public void SetDrawingVisual(DrawingVisual visual)
    {
        if (_drawingVisual != null)
        {
            _children.Remove(_drawingVisual);
        }

        _drawingVisual = visual;
        _children.Add(_drawingVisual);
    }

    protected override int VisualChildrenCount => _children.Count;

    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= _children.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return _children[index];
    }
}