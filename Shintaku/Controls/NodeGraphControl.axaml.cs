using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Shintaku.Controls;

public class NodeGraphControl : TemplatedControl
{
    private Canvas? _canvas;
    private bool _isCanvasDragging;
    private Point _oldPointerPos;

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
    }

    private void CanvasOnPointerMoved(object sender, PointerEventArgs e)
    {
        
        
    }

    private void CanvasOnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isCanvasDragging = false;
    }

    private void CanvasOnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        _oldPointerPos = e.GetPosition(_canvas);
        _isCanvasDragging = true;
    }
}