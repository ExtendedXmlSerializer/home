using System;

namespace VweCore.Geometry
{
    public readonly struct Point2D : IEquatable<Point2D>
    {
        public static readonly Point2D Zero = new Point2D(0.0, 0.0);

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public bool Equals(Point2D other) => X.IsApproximately(other.X) && Y.IsApproximately(other.Y);

        public override bool Equals(object? obj) => obj is Point2D point && Equals(point);

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.Round().GetHashCode() * 397) ^ Y.Round().GetHashCode();
            }
        }

        public override string ToString() => $"X: {X.ToInvariantString()} Y: {Y.ToInvariantString()}";

        public Point2D Add(Point2D other) => new Point2D(X + other.X, Y + other.Y);

        public Point2D Subtract(Point2D other) => new Point2D(X - other.X, Y - other.Y);

        public Point2D Multiply(Point2D other) => new Point2D(X * other.X, Y * other.Y);

        public Point2D Multiply(double factor) => new Point2D(X * factor, Y * factor);

        public static Point2D Negate(Point2D point) => new Point2D(-point.X, -point.Y);

        public Point2D RotateAroundOrigin(double angleInDegrees)
        {
            var angleInRadians = angleInDegrees.ToRadians();
            var newX = CalculateRotatedXCoordinate(angleInRadians);
            var newY = CalculateRotatedYCoordinate(angleInRadians);
            return new Point2D(newX, newY);
        }

        public Point2D RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            if (Equals(referencePoint))
                return this;

            return TranslateToOrigin(referencePoint)
                  .RotateAroundOrigin(angleInDegrees)
                  .TranslateBackFromOrigin(referencePoint);
        }

        public Point2D TranslateToOrigin(Point2D referencePoint) =>
            new Point2D(X - referencePoint.X, Y - referencePoint.Y);

        public Point2D TranslateBackFromOrigin(Point2D referencePoint) =>
            new Point2D(X + referencePoint.X, Y + referencePoint.Y);

        public double CalculateRotatedXCoordinate(double angleInRadians) => X * Math.Cos(angleInRadians) - Y * Math.Sin(angleInRadians);

        public double CalculateRotatedYCoordinate(double angleInRadians) => X * Math.Sin(angleInRadians) + Y * Math.Cos(angleInRadians);

        public static bool operator !=(Point2D left, Point2D right) => !left.Equals(right);

        public static bool operator ==(Point2D left, Point2D right) => left.Equals(right);

        public static Point2D operator +(Point2D left, Point2D right) => left.Add(right);

        public static Point2D operator -(Point2D left, Point2D right) => left.Subtract(right);

        public static Point2D operator *(Point2D left, Point2D right) => left.Multiply(right);

        public static Point2D operator *(Point2D left, double right) => left.Multiply(right);

        public static Point2D operator *(double left, Point2D right) => right.Multiply(left);

        public static Point2D operator -(Point2D point) => Negate(point);
    }
}