
using Avalonia;

namespace QuadTrees.QTreePoint
{
    /// <summary>
    /// Interface to define AbsoluteRect, so that QuadTree knows how to store the object.
    /// </summary>
    public interface IPointQuadStorable
    {
        /// <summary>
        /// The Point that defines the object's boundaries.
        /// </summary>
        Point Point { get; }
    }
}