using VweCore.Geometry;

namespace VweCore.Abstractions
{
    public interface IMovable : IEntity
    {
        void Move(Point2D moveVector);
    }
}