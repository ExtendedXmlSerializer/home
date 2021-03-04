using VweCore.Abstractions;
using VweCore.Geometry;

namespace VweCore
{
    public sealed class HallwayNode : Node, IPositionable, IRelativelyRotatable
    {
        public Point2D Position { get; set; }

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        void IRelativelyRotatable.RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint) =>
            RotateAroundReferencePoint(angleInDegrees, referencePoint);

        public HallwayNode Move(Point2D moveVector)
        {
            Position += moveVector;
            return this;
        }

        public HallwayNode Clone(int id) =>
            new HallwayNode
            {
                Id = id,
                DirectionalAngleInDegrees = DirectionalAngleInDegrees,
                Radius = Radius,
                Position = Position
            };

        public override Point2D GetAbsolutePosition() => Position;

        public HallwayNode RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            Position = Position.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            return this;
        }

        public override string ToString() => $"{TypeIdentifier} {Id} ({Position})";
    }
}