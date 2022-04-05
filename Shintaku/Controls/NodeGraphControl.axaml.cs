using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ReactiveUI;
using System;

namespace Shintaku.Controls;

public class NodeGraphControl : TemplatedControl
{
    private Canvas? _canvas;
    private bool _isCanvasDragging;
    private Point _oldPointerPos;
    private Rect _viewPort = Rect.Empty;

    public static readonly DirectProperty<NodeGraphControl, Rect> ViewPortProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, Rect>("ViewPort", o => o.ViewPort, (o, v) => o.ViewPort = v);

    public Rect ViewPort
    {
        get => _viewPort;
        set => SetAndRaise(ViewPortProperty, ref _viewPort, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _canvas = e.NameScope.Find<Canvas>("PART_Canvas");

        if (_canvas is null)
        {
            return;
        }

        _canvas.PointerPressed += CanvasOnPointerPressed;
        _canvas.PointerReleased += CanvasOnPointerReleased;
        _canvas.PointerMoved += CanvasOnPointerMoved;

        _canvas.WhenAnyValue(x => x.Bounds)
            .Subscribe(CanvasBoundsChanged);
    }

    private void CanvasBoundsChanged(Rect currentBounds)
    {
        ViewPort = new Rect(_viewPort.X, _viewPort.Y, currentBounds.Width, currentBounds.Height);
    }

    private void CanvasOnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isCanvasDragging) return;
        
        var curPointerPos = e.GetPosition(_canvas);
        var delta = _oldPointerPos - curPointerPos;
        _oldPointerPos = curPointerPos;
        var currentViewPos = _viewPort.Position;
        currentViewPos += delta;
        ViewPort = new Rect(currentViewPos.X, currentViewPos.Y, _viewPort.Width, _viewPort.Height);
    }

    private void CanvasOnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Cursor = new Cursor(StandardCursorType.Arrow);
        _isCanvasDragging = false;
    }

    private void CanvasOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Cursor = new Cursor(StandardCursorType.Hand);
        _oldPointerPos = e.GetPosition(_canvas);
        _isCanvasDragging = true;
    }
}