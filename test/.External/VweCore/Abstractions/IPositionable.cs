using VweCore.Geometry;

namespace VweCore.Abstractions
{
    public interface IPositionable : IMovable
    {
        Point2D Position { get; set; }
    }
}