using VweCore.Geometry;

namespace VweCore.Abstractions
{
    public interface ILineSegment : IEntity
    {
        Point2D Point1 { get; set; }

        Point2D Point2 { get; set; }
    }
}