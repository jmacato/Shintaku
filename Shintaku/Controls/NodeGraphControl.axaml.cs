using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Avalonia.Animation;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Shintaku.ViewModels.Nodes;

namespace Shintaku.Controls;

public class NodeGraphControl : TemplatedControl
{
    private Canvas? _canvas;
    private bool _isCanvasDragging;
    private Point _oldPointerPos;

    private Rect _virtualViewPort = Rect.Empty;
    private List<NodeViewModel> _visibleNodes = new();
    private double _scrollDelta = 100;
    private double _zoomScalar = 1;

    public static readonly DirectProperty<NodeGraphControl, Rect> VirtualViewPortProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, Rect>(nameof(VirtualViewPort),
            o => o.VirtualViewPort,
            (o, v) => o.VirtualViewPort = v);

    public static readonly DirectProperty<NodeGraphControl, List<NodeViewModel>> VisibleNodesProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, List<NodeViewModel>>(nameof(VisibleNodes),
            o => o.VisibleNodes,
            (o, v) => o.VisibleNodes = v);


    public static readonly DirectProperty<NodeGraphControl, double> ZoomScalarProperty
        = AvaloniaProperty.RegisterDirect<NodeGraphControl, double>("ZoomScalar",
            o => o.ZoomScalar,
            (o, v) => o.ZoomScalar = v);

    private Size _actualViewPortSize;

    public static readonly DirectProperty<NodeGraphControl, Size> ActualViewPortSizeProperty
        = AvaloniaProperty.RegisterDirect<NodeGraphControl, Size>("ActualViewPortSize",
            o => o.ActualViewPortSize,
            (o, v) => o.ActualViewPortSize = v);

    private Matrix _viewPortMatrix;

    public static readonly DirectProperty<NodeGraphControl, Matrix> ViewPortMatrixProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, Matrix>("ViewPortMatrix",
            o => o.ViewPortMatrix,
            (o, v) => o.ViewPortMatrix = v);

    private Point _currentPointerPos;

    public Rect VirtualViewPort
    {
        get => _virtualViewPort;
        set => SetAndRaise(VirtualViewPortProperty, ref _virtualViewPort, value);
    }

    public List<NodeViewModel> VisibleNodes
    {
        get => _visibleNodes;
        set => SetAndRaise(VisibleNodesProperty, ref _visibleNodes, value);
    }

    public double ZoomScalar
    {
        get => _zoomScalar;
        set => SetAndRaise(ZoomScalarProperty, ref _zoomScalar, value);
    }

    public Size ActualViewPortSize
    {
        get => _actualViewPortSize;
        set => SetAndRaise(ActualViewPortSizeProperty, ref _actualViewPortSize, value);
    }

    public Matrix ViewPortMatrix
    {
        get => _viewPortMatrix;
        set => SetAndRaise(ViewPortMatrixProperty, ref _viewPortMatrix, value);
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
        _canvas.PointerWheelChanged += CanvasOnPointerWheelChanged;

        _canvas.WhenAnyValue(x => x.Bounds)
            .Subscribe(CanvasBoundsChanged);

        this.WhenAnyValue(x => x.VisibleNodes)
            .Subscribe(NewVisibleNodes);
    }


    private void CanvasOnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        bool zoomOnCursor = false;

        var curPos = e.GetPosition(_canvas) + _virtualViewPort.Position;

        _scrollDelta = Math.Clamp(_scrollDelta + e.Delta.Y, -300, 300);
        ZoomScalar = Math.Clamp(_scrollDelta / 100, 0.1, 3);

        UpdateMatrixAndViewPort();
    }

    private void UpdateMatrixAndViewPort()
    {
        var scaleOrigin = _actualViewPortSize / 2;
        
        ViewPortMatrix = Matrix.Identity *
                         Matrix.CreateTranslation(_currentPointerPos.X, _currentPointerPos.Y) *
                         ScaleAt(  _zoomScalar,   _zoomScalar, scaleOrigin.Width, scaleOrigin.Height);
        
        var virtualRect = new Rect(0, 0, _actualViewPortSize.Width, _actualViewPortSize.Height);

        VirtualViewPort = virtualRect.TransformToAABB(_viewPortMatrix.Invert());
    }


    public Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix(scaleX, 0,
            0, scaleY,
            centerX - scaleX * centerX, centerY - scaleY * centerY);
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

            var transformedRect = nodeViewModel.Rect.TransformToAABB(_viewPortMatrix);

            Canvas.SetLeft(newContent, transformedRect.X);
            Canvas.SetTop(newContent, transformedRect.Y);
            newContent.Width = transformedRect.Width;
            newContent.Height = transformedRect.Height;
            newContent.Background = Brushes.Aqua;

            _canvas.Children.Add(newContent);
        }
    }


    private void CanvasBoundsChanged(Rect currentBounds)
    {
        ActualViewPortSize = currentBounds.Size;
        UpdateMatrixAndViewPort();
    }

    private void CanvasOnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isCanvasDragging) return;

        var newPointerPos = e.GetPosition(_canvas);
        var delta = _oldPointerPos - newPointerPos;
        _oldPointerPos = newPointerPos;
        _currentPointerPos -= delta * (1 / _zoomScalar);
        UpdateMatrixAndViewPort();
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