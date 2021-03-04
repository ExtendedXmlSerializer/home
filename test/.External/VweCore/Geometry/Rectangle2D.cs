using System;
using Light.GuardClauses;

namespace VweCore.Geometry
{
    public readonly struct Rectangle2D : IEquatable<Rectangle2D>
    {
        public Rectangle2D(Point2D position, double width, double height, double angleInDegrees)
        {
            Position = position;
            Width = width.MustNotBeLessThan(0.0, nameof(width));
            Height = height.MustNotBeLessThan(0.0, nameof(height));
            AngleInDegrees = angleInDegrees;
        }

        public Point2D Position { get; }

        public double Width { get; }

        public double Height { get; }

        public double AngleInDegrees { get; }

        public bool Equals(Rectangle2D other) =>
            Position.Equals(other.Position) &&
            Width.IsApproximately(other.Width) &&
            Height.IsApproximately(other.Height) &&
            AngleInDegrees.IsApproximately(other.AngleInDegrees);

        public override bool Equals(object? obj) => obj is Rectangle2D other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.Round().GetHashCode();
                hashCode = (hashCode * 397) ^ Height.Round().GetHashCode();
                hashCode = (hashCode * 397) ^ AngleInDegrees.Round().GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Rectangle2D left, Rectangle2D right) => left.Equals(right);

        public static bool operator !=(Rectangle2D left, Rectangle2D right) => !left.Equals(right);

        public bool Contains(Circle2D circle)
        {
            var normalizedCircle = circle.Move(-Position).RotateAroundOrigin(-AngleInDegrees);
            var radius = circle.Diameter / 2;
            var normalizedCirclePosition = normalizedCircle.Position;

            var rightBoundary = Width / 2;
            var leftBoundary = -rightBoundary;
            var topBoundary = Height / 2;
            var bottomBoundary = -topBoundary;

            var circleLeft = normalizedCirclePosition.X - radius;
            var circleRight = normalizedCirclePosition.X + radius;
            var circleTop = normalizedCirclePosition.Y + radius;
            var circleBottom = normalizedCirclePosition.Y - radius;

            return circleLeft.IsGreaterThanOrApproximatelyEqualTo(leftBoundary) &&
                   circleRight.IsLessThanOrApproximatelyEqualTo(rightBoundary) &&
                   circleTop.IsLessThanOrApproximatelyEqualTo(topBoundary) &&
                   circleBottom.IsGreaterThanOrApproximatelyEqualTo(bottomBoundary);
        }

        public override string ToString() => $"Rectangle (Position: {Position}, Width {Width.ToInvariantString()}, Height {Height.ToInvariantString()}, Angle {AngleInDegrees.ToInvariantString()}°)";
    }
}