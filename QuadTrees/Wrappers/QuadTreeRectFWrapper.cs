using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using QuadTrees.QTreeRectF;

namespace QuadTrees.Wrappers
{
    /// <summary>
    /// A simple container for a rectangle in a QuadTree
    /// </summary>
    public struct QuadTreeRectFWrapper : IRectFQuadStorable
    {
        private Rect _rect;

        public Rect Rect => _rect;

        public QuadTreeRectFWrapper(Rect rect)
        {
            _rect = rect;
        }
    }
}
