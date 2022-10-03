using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QuadTrees;
using QuadTrees.QTreeRectF;
using ReactiveUI;

namespace Shintaku.ViewModels.Nodes;

public partial class NodeGraphViewModel : ViewModelBase
{
    [ObservableProperty] private List<IRectFQuadStorable> _allNodes = new();
    [ObservableProperty] private Rect _currentViewPort;
    [ObservableProperty] private double _zoomScalar = 1d;

    private readonly Random _rnd = new Random();

    public NodeGraphViewModel()
    {
        var tempQuadTree = new QuadTreeRectF<IRectFQuadStorable>();
        for (int i = 0; i < 100; i++)
        {
            var kX = _rnd.NextDouble() * 2000;
            var kY = _rnd.NextDouble() * 2000;
            var kW = Math.Max(25, _rnd.NextDouble() * 100);
            var kH = Math.Max(25, _rnd.NextDouble() * 100);

            var candidate = new Rect(kX, kY, kW, kH);

            if (tempQuadTree.GetObjects(candidate.Inflate(10)).Count > 0)
            {
                i--;
                continue;
            }

            tempQuadTree.Add(new NodeViewModel(candidate, i));
        }
        AllNodes = tempQuadTree.GetAllObjects().ToList();
        tempQuadTree.Clear();
    }
}