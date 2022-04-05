using Avalonia;
using QuadTrees.Common;
using QuadTrees.QTreeRectF;

namespace QuadTrees
{
    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of Rectangles in a world space, queried using Rectangles.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTreeRectF<T> : QuadTreeFCommon<T, QuadTreeRectNode<T, Rect>, Rect> where T : IRectFQuadStorable
    {
        public QuadTreeRectF(Rect rect) : base(rect)
        {
        }

        public QuadTreeRectF(double x, double y, double width, double height) : base(x, y, width, height)
        {
        }

        protected override QuadTreeRectNode<T, Rect> CreateNode(Rect rect)
        {
            return new QuadTreeRectFNode<T>(rect);
        }
    }
}
