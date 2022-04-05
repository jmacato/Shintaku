using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using QuadTrees;
using ReactiveUI;

namespace Shintaku.ViewModels.Nodes;

public partial class NodeGraphViewModel : ViewModelBase
{
    [ObservableProperty] private List<NodeViewModel> _visibleNodes;
    [ObservableProperty] private Rect _currentViewPort;
    private readonly Random _rnd = new Random();
    private readonly QuadTreeRectF<NodeViewModel> _nodes = new ();

    public NodeGraphViewModel()
    {
        for (int i = 0; i < 20; i++)
        {
            var kX = _rnd.NextDouble() * 1000;
            var kY = _rnd.NextDouble() * 1000;
            var kW = _rnd.NextDouble() * 1000;
            var kH = _rnd.NextDouble() * 1000;
            
            _nodes.Add(new NodeViewModel(new Rect(kX, kY, kW, kH)));
        }

        this.WhenAnyValue(x => x.CurrentViewPort)
            .Subscribe(ViewPortUpdated);
    }

    private void ViewPortUpdated(Rect curView)
    {
        var currentNodes = _nodes.GetObjects(curView);
        Debug.WriteLine($"ViewPort: {curView} / Visible Objects {currentNodes.Count}/{_nodes.Count}");
        VisibleNodes = currentNodes;
    }
}