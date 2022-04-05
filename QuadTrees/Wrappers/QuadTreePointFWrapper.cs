using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using QuadTrees.QTreePoint;

namespace QuadTrees.Wrappers
{
    /// <summary>
    /// A simple container for a point in a QuadTree
    /// </summary>
    public struct QuadTreePointWrapper: IPointQuadStorable
    {
        private Point _point;

        public Point Point => _point;

        public QuadTreePointWrapper(Point point)
        {
            _point = point;
        }
    }
}
