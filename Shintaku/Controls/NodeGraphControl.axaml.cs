using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Media;
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
            .Buffer(2, 1)
            .Select(b => (Previous: b[0], Current: b[1]))
            .Subscribe(b => NewVisibleNodes(b.Previous, b.Current));
    }


    private void CanvasOnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var curPos = e.GetPosition(_canvas) + _virtualViewPort.Position;

        _scrollDelta = Math.Clamp(_scrollDelta + e.Delta.Y, -100, 100);

        ZoomScalar = MapRange(_scrollDelta, -100, 100, 0.3, 2);

        UpdateMatrixAndViewPort();
    }

    private static double MapRange(
        double value,
        double sourceMin,
        double sourceMax,
        double targetMin,
        double targetMax)
    {
        return (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
    }

    private void UpdateMatrixAndViewPort()
    {
        var scaleOrigin = _actualViewPortSize / 2;

        ViewPortMatrix = Matrix.Identity *
                         Matrix.CreateTranslation(_currentPointerPos.X, _currentPointerPos.Y) *
                         ScaleAt(_zoomScalar, _zoomScalar, scaleOrigin.Width, scaleOrigin.Height);

        var virtualRect = new Rect(0, 0, _actualViewPortSize.Width, _actualViewPortSize.Height);

        VirtualViewPort = virtualRect.TransformToAABB(_viewPortMatrix.Invert());
    }


    public Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix(scaleX, 0,
            0, scaleY,
            centerX - scaleX * centerX, centerY - scaleY * centerY);
    }

    private void NewVisibleNodes(List<NodeViewModel> previousList, List<NodeViewModel> newList)
    {
        if (_canvas is null)
        {
            return;
        }

        var removedObjects = 0;
        var addedObjects = 0;

        var currentRect = new Rect(new Point(), ActualViewPortSize);

        foreach (var nodeViewModel in newList.Where(x => !previousList.Contains(x)))
        {
            var transformedRect = nodeViewModel.Rect.TransformToAABB(_viewPortMatrix);

            if (!currentRect.Intersects(transformedRect))
            {
                continue;
            }

            var newContent = new ContentControl();
            newContent.DataContext = nodeViewModel;

            Canvas.SetLeft(newContent, transformedRect.X);
            Canvas.SetTop(newContent, transformedRect.Y);
            newContent.Width = transformedRect.Width;
            newContent.Height = transformedRect.Height;
            newContent.Background = Brushes.Aqua;
            _canvas.Children.Add(newContent);
            addedObjects++;
        }

        foreach (var nodeViewModel in previousList.Where(newList.Contains))
        {
            var t = _canvas.Children
                .Select(x => (x as Control, x.DataContext as NodeViewModel))
                .Where(x => x.Item1 is not null && x.Item2 is not null)
                .ToList();
            
            foreach (var (u, v) in t)
            {
                if (v is null || u is null)
                {
                    return;
                }

                var transformedRect = v.Rect.TransformToAABB(_viewPortMatrix);

                if (!currentRect.Intersects(transformedRect))
                {
                    _canvas.Children.Remove(u);
                    removedObjects++;
                }

                Canvas.SetLeft(u, transformedRect.X);
                Canvas.SetTop(u, transformedRect.Y);
                u.Width = transformedRect.Width;
                u.Height = transformedRect.Height;
            }
        }

        if (removedObjects > 0 || addedObjects > 0)
            Debug.WriteLine($"Objects removed: {removedObjects} / added: {addedObjects}");
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