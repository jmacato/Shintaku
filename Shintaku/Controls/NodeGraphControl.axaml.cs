using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Avalonia.Media;
using Shintaku.ViewModels.Nodes;

namespace Shintaku.Controls;

public class NodeGraphControl : TemplatedControl
{
    private Canvas? _canvas;
    private bool _isCanvasDragging;
    private Point _oldPointerPos;
    
    private Rect _viewPort = Rect.Empty;
    private List<NodeViewModel> _visibleNodes = new();

    public static readonly DirectProperty<NodeGraphControl, Rect> ViewPortProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, Rect>(nameof(ViewPort),
            o => o.ViewPort,
            (o,
                v) => o.ViewPort = v);
    
    public static readonly DirectProperty<NodeGraphControl, List<NodeViewModel>> VisibleNodesProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, List<NodeViewModel>>(nameof(VisibleNodes),
            o => o.VisibleNodes,
            (o,
                v) => o.VisibleNodes = v);

    public Rect ViewPort
    {
        get => _viewPort;
        set => SetAndRaise(ViewPortProperty, ref _viewPort, value);
    }

    public List<NodeViewModel> VisibleNodes
    {
        get => _visibleNodes;
        set => SetAndRaise(VisibleNodesProperty, ref _visibleNodes, value);
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

        this.WhenAnyValue(x => x.VisibleNodes)
            .Subscribe(NewVisibleNodes);
    }

    private void NewVisibleNodes(List<NodeViewModel> newList)
    {
        if (_canvas is null)
        {
            return;
        }
        
        _canvas.Children.Clear();

        foreach (var nodeViewModel in newList)
        {
            var newContent = new Panel();
            newContent.DataContext = nodeViewModel;
             
            var finalPos = nodeViewModel.Rect.Position - _viewPort.Position;
            Canvas.SetLeft(newContent, finalPos.X);
            Canvas.SetTop(newContent, finalPos.Y);
            newContent.Width = nodeViewModel.Rect.Width;
            newContent.Height = nodeViewModel.Rect.Height;
            newContent.Background= Brushes.Aqua;
            
            _canvas.Children.Add(newContent);
        }
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