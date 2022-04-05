using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using QuadTrees.Common;
using QuadTrees.QTreePoint;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Points in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePoint<T> : QuadTreeFCommon<T, QuadTreePointNode<T>, Rect> where T : IPointQuadStorable
    {
        public QuadTreePoint(Rect rect)
            : base(rect)
        {
        }

        public QuadTreePoint(double x, double y, double width, double height)
            : base(x, y, width, height)
        {
        }

        protected override QuadTreePointNode<T> CreateNode(Rect rect)
        {
            return new QuadTreePointNode<T>(rect);
        }
    }
}
