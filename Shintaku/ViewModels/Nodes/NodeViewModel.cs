using System.ComponentModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using QuadTrees;
using QuadTrees.QTreeRectF;

namespace Shintaku.ViewModels.Nodes;

public partial class NodeViewModel : ViewModelBase, IRectFQuadStorable
{
    public int ID { get; }

    [ObservableProperty] private Rect _absoluteRect;
    [ObservableProperty] private Rect _viewportRect;
    [ObservableProperty] private double _left;
    [ObservableProperty] private double _top;
    [ObservableProperty] private double _width;
    [ObservableProperty] private double _height;

    public NodeViewModel(Rect rect, int id)
    {
        AbsoluteRect = rect;
        ID = id;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewportRect))
        {
            UpdateRects(ViewportRect);
        }

        base.OnPropertyChanged(e);
    }

    private void UpdateRects(Rect rect)
    {
        Left = rect.Left;
        Top = rect.Top;
        Width = rect.Width;
        Height = rect.Height;
    }
}