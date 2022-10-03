
using Avalonia;

namespace QuadTrees.QTreeRectF
{
    /// <summary>
    /// Interface to define AbsoluteRect, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IRectFQuadStorable
    {
        /// <summary>
        /// The AbsoluteRect that defines the object's boundaries.
        /// </summary>
        Rect AbsoluteRect { get; }
        Rect ViewportRect { get; set; }

    }
}