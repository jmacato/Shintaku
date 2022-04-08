using System;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using QuadTrees;
using QuadTrees.QTreeRectF;

namespace Shintaku.ViewModels.Nodes;

public partial class NodeViewModel : ViewModelBase, IRectFQuadStorable
{
    [ObservableProperty] private Rect _rect;
    
    
     public Guid Guid { get; }
    
    public NodeViewModel(Rect rect)
    {
        _rect = rect;
        Guid = Guid.NewGuid();
    }
}