using Avalonia;
using QuadTrees.Common;

namespace QuadTrees.QTreeRectF
{

    public class QuadTreeRectPointInvNode<T> : QuadTreeRectNode<T, Point> where T : IRectFQuadStorable
    {
        public QuadTreeRectPointInvNode(Rect rect) : base(rect)
        {
        }

        public QuadTreeRectPointInvNode(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        internal QuadTreeRectPointInvNode(QuadTreeRectNode<T, Point> parent, Rect rect)
            : base(parent, rect)
        {
        }
        protected override QuadTreeRectNode<T, Point> CreateNode(Rect Rect)
        {
            VerifyNodeAssertions(Rect);
            return new QuadTreeRectPointInvNode<T>(this, Rect);
        }

        protected override bool CheckIntersects(Point searchRect, T data)
        {
            return data.Rect.Contains(searchRect);
        }

        public override bool ContainsObject(QuadTreeObject<T, QuadTreeRectNode<T, Point>> qto)
        {
            return CheckContains(QuadRect, qto.Data);
        }

        protected override bool QueryContains(Point search, Rect rect)
        {
            return false;
        }

        protected override bool QueryIntersects(Point search, Rect rect)
        {
            return rect.Contains(search);
        }

        protected override Point GetMortonPoint(T p)
        {
            return p.Rect.Position;
        }
    }
}