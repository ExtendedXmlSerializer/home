using VweCore.Geometry;

namespace VweCore.Abstractions
{
    public interface IRelativelyPositionable : IMovable
    {
        Point2D Offset { get; set; }
    }
}