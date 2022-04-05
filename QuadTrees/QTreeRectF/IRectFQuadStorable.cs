
using Avalonia;

namespace QuadTrees.QTreeRectF
{
    /// <summary>
    /// Interface to define Rect, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IRectFQuadStorable
    {
        /// <summary>
        /// The Rect that defines the object's boundaries.
        /// </summary>
        Rect Rect { get; }
    }
}