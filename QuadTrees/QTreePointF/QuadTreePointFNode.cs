using System.Diagnostics;
using Avalonia;
using QuadTrees.Common;
using QuadTrees.QTreeRectF;

namespace QuadTrees.QTreePoint
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreePointNode<T> : QuadTreeFNodeCommon<T, QuadTreePointNode<T>> where T : IPointQuadStorable
    {
        public QuadTreePointNode(Rect rect)
            : base(rect)
        {
        }

        public QuadTreePointNode(double x, double y, double width, double height)
            : base(x, y, width, height)
        {
        }

        internal QuadTreePointNode(QuadTreePointNode<T> parent, Rect rect)
            : base(parent, rect)
        {
        }

        protected override QuadTreePointNode<T> CreateNode(Rect rect)
        {
            VerifyNodeAssertions(rect);
            return new QuadTreePointNode<T>(this, rect);
        }

        protected override bool CheckContains(Rect rect, T data)
        {
            return rect.Contains(data.Point);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreePointNode<T>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool CheckIntersects(Rect searchRect, T data)
        {
            return CheckContains(searchRect, data);
        }

        protected override Point GetMortonPoint(T p)
        {
            return p.Point;
        }
    }
}