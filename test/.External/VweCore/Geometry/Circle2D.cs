using System;
using Light.GuardClauses;

namespace VweCore.Geometry
{
    public readonly struct Circle2D : IEquatable<Circle2D>
    {
        public Circle2D(Point2D position, double diameter)
        {
            Position = position;
            Diameter = diameter.MustNotBeLessThan(0.0, nameof(diameter));
        }

        public Point2D Position { get; }

        public double Diameter { get; }

        public bool Equals(Circle2D other) => Position.Equals(other.Position) && Diameter.IsApproximately(other.Diameter);

        public override bool Equals(object? obj) => obj is Circle2D other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ Diameter.GetHashCode();
            }
        }

        public static bool operator ==(Circle2D left, Circle2D right) => left.Equals(right);

        public static bool operator !=(Circle2D left, Circle2D right) => !left.Equals(right);

        public Circle2D Move(Point2D vector) => new Circle2D(Position + vector, Diameter);

        public Circle2D RotateAroundOrigin(double angleInDegrees) => new Circle2D(Position.RotateAroundOrigin(angleInDegrees), Diameter);

        public override string ToString() => $"Circle (Position {Position}, Diameter {Diameter.ToInvariantString()})";
    }
}