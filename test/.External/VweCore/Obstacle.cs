using System;
using VweCore.Abstractions;
using VweCore.Geometry;
using VweCore.Translations;

namespace VweCore
{
    public sealed class Obstacle : Entity<Obstacle>, IMovable, IRelativelyRotatable, IEquatable<Obstacle>, IComparable<Obstacle>, ILineSegment
    {
        public Obstacle() : base(EntityOrderIndex.Obstacles, TranslationKeys.EntityName_Obstacle) { }

        public Point2D Point1 { get; set; }

        public Point2D Point2 { get; set; }

        public override LeveledRectangle GetBounds() =>
            new LeveledRectangle(Point1, Point2);

        void IMovable.Move(Point2D moveVector) => Move(moveVector);

        void IRelativelyRotatable.RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint) =>
            RotateAroundReferencePoint(angleInDegrees, referencePoint);

        public Obstacle Move(Point2D moveVector)
        {
            Point1 += moveVector;
            Point2 += moveVector;
            return this;
        }

        public Obstacle RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            Point1 = Point1.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            Point2 = Point2.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            return this;
        }

        public Point2D CalculateCenterPoint()
        {
            var halfDifference = (Point2 - Point1) * 0.5;
            return Point1 + halfDifference;
        }

        public Obstacle Clone(int id) =>
            new Obstacle
            {
                Id = id,
                Point1 = Point1,
                Point2 = Point2
            };
    }
}