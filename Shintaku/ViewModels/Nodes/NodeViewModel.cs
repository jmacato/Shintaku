using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using QuadTrees;
using QuadTrees.QTreeRectF;

namespace Shintaku.ViewModels.Nodes;

public partial class NodeViewModel : ViewModelBase, IRectFQuadStorable
{
    [ObservableProperty] private Rect _rect;
    
    public int ID { get; }
    
    public NodeViewModel(Rect rect, int id)
    {
        _rect = rect;
        ID = id;
    }
}