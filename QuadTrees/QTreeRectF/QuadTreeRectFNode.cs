using Avalonia;
using QuadTrees.Common;
using QuadTrees.Helper;

namespace QuadTrees.QTreeRectF
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public abstract class QuadTreeRectNode<T, TQuery> : QuadTreeFNodeCommon<T, QuadTreeRectNode<T, TQuery>, TQuery> where T : IRectFQuadStorable
    {
        public QuadTreeRectNode(Rect rect) : base(rect)
        {
        }

        public QuadTreeRectNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectNode(QuadTreeRectNode<T, TQuery> parent, Rect rect) : base(parent, rect)
        {
        }

        protected override bool CheckContains(Rect Rect, T data)
        {
            return Rect.Contains(data.AbsoluteRect);
        }
    }

    public class QuadTreeRectFNode<T> : QuadTreeRectNode<T, Rect> where T : IRectFQuadStorable
    {
        public QuadTreeRectFNode(Rect rect) : base(rect)
        {
        }

        public QuadTreeRectFNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectFNode(QuadTreeRectFNode<T> parent, Rect rect) : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, Rect> CreateNode(Rect Rect)
        {
            VerifyNodeAssertions(Rect);
            return new QuadTreeRectFNode<T>(this, Rect);
        }

        protected override bool CheckIntersects(Rect searchRect, T data)
        {
            return searchRect.Intersects(data.AbsoluteRect);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreeRectNode<T, Rect>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool QueryContains(Rect search, Rect rect)
        {
            return search.Contains(rect);
        }

        protected override bool QueryIntersects(Rect search, Rect rect)
        {
            return search.Intersects(rect);
        }
        protected override Point GetMortonPoint(T p)
        {
            return p.AbsoluteRect.Position; 
        }
    }
}