using VweCore.Abstractions;
using VweCore.Geometry;

namespace VweCore
{
    public sealed class TagNode : Node, IPositionable, IRelativelyRotatable
    {
        public Point2D Position { get; set; }

        public ulong RfId { get; set; }

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        void IRelativelyRotatable.RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint) =>
            RotateAroundReferencePoint(angleInDegrees, referencePoint);

        public TagNode Move(Point2D moveVector)
        {
            Position += moveVector;
            return this;
        }

        public TagNode Clone(int id) =>
            new ()
            {
                Id = id,
                DirectionalAngleInDegrees = DirectionalAngleInDegrees,
                Radius = Radius,
                Position = Position,
                RfId = RfId
            };

        public override Point2D GetAbsolutePosition() => Position;

        public TagNode RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            Position = Position.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            return this;
        }

        public override string ToString() => $"{TypeIdentifier} {Id} ({Position})";
    }
}
