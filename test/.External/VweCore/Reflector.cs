using System;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

namespace VweCore
{
    public sealed class Reflector : Entity<Reflector>, IPositionable, IRelativelyRotatable, IEquatable<Reflector>, IComparable<Reflector>
    {
        public Reflector() : base(EntityOrderIndex.Reflectors, TranslationKeys.EntityName_Reflector) { }

        public Point2D Position { get; set; }

        public override LeveledRectangle GetBounds() =>
            new LeveledRectangle(Position, Position);

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        void IRelativelyRotatable.RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint) =>
            RotateAroundReferencePoint(angleInDegrees, referencePoint);

        public void SetAbsolutePosition(Point2D position) => Position = position;

        public Reflector Move(Point2D moveVector)
        {
            Position += moveVector;
            return this;
        }

        public Reflector RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            Position = Position.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            return this;
        }

        public Reflector Clone(int id) =>
            new Reflector
            {
                Id = id,
                Position = Position
            };
    }
}