using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using QuadTrees;
using QuadTrees.QTreeRectF;

namespace Shintaku.Controls;

public class NodeGraphControl : TemplatedControl
{
    private Panel? _touchPanel;
    private bool _isCanvasDragging;
    private Point _oldPointerPos;

    private IEnumerable<IRectFQuadStorable> _visibleNodes = Enumerable.Empty<IRectFQuadStorable>();

    private double _scrollDelta = 100;
    private double _zoomScalar = 1;

    public static readonly DirectProperty<NodeGraphControl, IEnumerable<IRectFQuadStorable>>
        VisibleNodesProperty =
            AvaloniaProperty.RegisterDirect<NodeGraphControl, IEnumerable<IRectFQuadStorable>>(
                nameof(VisibleNodes),
                o => o.VisibleNodes,
                (o, v) => o.VisibleNodes = v);

    private Point _currentPointerPos;

    public IEnumerable<IRectFQuadStorable> VisibleNodes
    {
        get => _visibleNodes;
        set => SetAndRaise(VisibleNodesProperty, ref _visibleNodes, value);
    }

    private IEnumerable<IRectFQuadStorable> _nodes = Enumerable.Empty<IRectFQuadStorable>();

    public static readonly DirectProperty<NodeGraphControl, IEnumerable<IRectFQuadStorable>> NodesProperty =
        AvaloniaProperty.RegisterDirect<NodeGraphControl, IEnumerable<IRectFQuadStorable>>("Nodes",
            o => o.Nodes,
            (o, v) => o.Nodes = v);

    private Size _actualViewPortSize;

    public IEnumerable<IRectFQuadStorable> Nodes
    {
        get => _nodes;
        set => SetAndRaise(NodesProperty, ref _nodes, value);
    }

    private readonly QuadTreeRectF<IRectFQuadStorable> _quadTree = new();


    /// <inheritdoc />
    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        if (change.Property == NodesProperty && change.NewValue.HasValue &&
            change.NewValue.Value is IEnumerable<IRectFQuadStorable> val)
        {
            _quadTree.Clear();
            _quadTree.AddRange(val.ToList());
        }

        base.OnPropertyChanged(change);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _touchPanel = e.NameScope.Find<Panel>("PART_Panel")!;

        _touchPanel.PointerPressed += TouchPanelOnPointerPressed;
        _touchPanel.PointerReleased += TouchPanelOnPointerReleased;
        _touchPanel.PointerMoved += TouchPanelOnPointerMoved;
        _touchPanel.PointerWheelChanged += TouchPanelOnPointerWheelChanged;

        _touchPanel.WhenAnyValue(x => x.Bounds)
            .Subscribe(CanvasBoundsChanged);
    }


    private void TouchPanelOnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _scrollDelta = Math.Clamp(_scrollDelta + e.Delta.Y, -300, 300);
        _zoomScalar = Math.Clamp(_scrollDelta / 100, 0.1, 3);
        UpdateMatrixAndViewPort();
    }

    private void UpdateMatrixAndViewPort()
    {
        var scaleOrigin = _actualViewPortSize / 2;

        var viewPortMatrix = Matrix.Identity *
                             Matrix.CreateTranslation(_currentPointerPos.X, _currentPointerPos.Y) *
                             ScaleAt(_zoomScalar, _zoomScalar, scaleOrigin.Width, scaleOrigin.Height);

        var virtualRect = new Rect(0, 0, _actualViewPortSize.Width, _actualViewPortSize.Height);
        var virtualViewPort = virtualRect.TransformToAABB(viewPortMatrix.Invert());
        var visibleNodes = _quadTree.GetObjects(virtualViewPort).ToList();
        visibleNodes.ForEach(x => x.ViewportRect = x.AbsoluteRect.TransformToAABB(viewPortMatrix));
        VisibleNodes = visibleNodes;
    }


    public Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Matrix(scaleX, 0,
            0, scaleY,
            centerX - scaleX * centerX, centerY - scaleY * centerY);
    }

    private void CanvasBoundsChanged(Rect currentBounds)
    {
        _actualViewPortSize = currentBounds.Size;
        UpdateMatrixAndViewPort();
    }

    private void TouchPanelOnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isCanvasDragging) return;

        var newPointerPos = e.GetPosition(_touchPanel);
        var delta = _oldPointerPos - newPointerPos;
        _oldPointerPos = newPointerPos;
        _currentPointerPos -= delta * (1 / _zoomScalar);
        UpdateMatrixAndViewPort();
    }


    private void TouchPanelOnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Cursor = new Cursor(StandardCursorType.Arrow);
        _isCanvasDragging = false;
    }

    private void TouchPanelOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Cursor = new Cursor(StandardCursorType.Hand);
        _oldPointerPos = e.GetPosition(_touchPanel);
        _isCanvasDragging = true;
    }
}