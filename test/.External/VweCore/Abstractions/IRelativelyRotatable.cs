using VweCore.Geometry;

namespace VweCore.Abstractions
{
    public interface IRelativelyRotatable : IEntity
    {
        void RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint);
    }
}