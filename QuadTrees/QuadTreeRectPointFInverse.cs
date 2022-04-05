using Avalonia;
using QuadTrees.Common;
using QuadTrees.QTreeRectF;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Rectangles in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRectPointInverse<T> : QuadTreeFCommon<T, QuadTreeRectNode<T, Point>, Point> where T : IRectFQuadStorable
    {
        public QuadTreeRectPointInverse(Rect rect) : base(rect)
        {
        }

        public QuadTreeRectPointInverse(double x, double y, double width, double height) : base(x, y, width, height)
        {
        }

        protected override QuadTreeRectNode<T, Point> CreateNode(Rect rect)
        {
            return new QuadTreeRectPointInvNode<T>(rect);
        }
    }
}
