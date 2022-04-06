using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QuadTrees;
using ReactiveUI;

namespace Shintaku.ViewModels.Nodes;

public partial class NodeGraphViewModel : ViewModelBase
{
    [ObservableProperty] private List<NodeViewModel> _visibleNodes;
    [ObservableProperty] private Rect _currentViewPort;
    [ObservableProperty] private double _zoomScalar = 1d;
    
    private readonly Random _rnd = new Random();
    private readonly QuadTreeRectF<NodeViewModel> _nodes = new ();

    public NodeGraphViewModel()
    {
        for (int i = 0; i < 100; i++)
        {
            var kX = _rnd.NextDouble() * 2000;
            var kY = _rnd.NextDouble() * 2000;
            var kW = Math.Max(25,_rnd.NextDouble() * 100);
            var kH = Math.Max(25,_rnd.NextDouble() * 100);

            var candidate = new Rect(kX, kY, kW, kH);

            if (_nodes.GetObjects(candidate.Inflate(10)).Count > 0)
            {
                i--;
                continue;
            }
            
            _nodes.Add(new NodeViewModel(candidate, i));
        }

        this.WhenAnyValue(x => x.CurrentViewPort, x=>x.ZoomScalar)
            .Subscribe(ViewPortUpdated);
        
        
         
    }

    private void ViewPortUpdated((Rect curView, double zoom) result)
    {
        var currentNodes = _nodes.GetObjects(result.curView);
        Debug.WriteLine($"ViewPort: {result.curView} / Visible Objects {currentNodes.Count}/{_nodes.Count}");
        VisibleNodes = currentNodes;
    }
}