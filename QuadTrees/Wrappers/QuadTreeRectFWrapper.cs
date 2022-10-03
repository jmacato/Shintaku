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
        public Rect AbsoluteRect { get;  } 
        public Rect ViewportRect { get; set; }

        public QuadTreeRectFWrapper(Rect rect)
        {
            AbsoluteRect = rect;
            ViewportRect = Rect.Empty;
        }
    }
}
