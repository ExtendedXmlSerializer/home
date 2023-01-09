using System;

namespace VweCore.Geometry
{
    public readonly struct LeveledRectangle : IEquatable<LeveledRectangle>
    {
        public LeveledRectangle(Point2D firstPoint, Point2D secondPoint)
        {
            var lowerLeftX = firstPoint.X;
            var lowerLeftY = firstPoint.Y;
            var upperRightX = secondPoint.X;
            var upperRightY = secondPoint.Y;
            if (secondPoint.X < firstPoint.X)
            {
                lowerLeftX = secondPoint.X;
                upperRightX = firstPoint.X;
            }

            if (secondPoint.Y < firstPoint.Y)
            {
                lowerLeftY = secondPoint.Y;
                upperRightY = firstPoint.Y;
            }

            LowerLeft = new Point2D(lowerLeftX, lowerLeftY);
            UpperRight = new Point2D(upperRightX, upperRightY);
        }

        public static LeveledRectangle Empty => new LeveledRectangle(Point2D.Zero, Point2D.Zero);

        public Point2D LowerLeft { get; }

        public Point2D UpperRight { get; }

        public double Width => UpperRight.X - LowerLeft.X;

        public double Height => UpperRight.Y - LowerLeft.Y;

        public bool IsEmpty => LowerLeft.Equals(UpperRight);

        public Point2D Center => new Point2D(LowerLeft.X + Width / 2, LowerLeft.Y + Height / 2);

        public bool Equals(LeveledRectangle other) => LowerLeft.Equals(other.LowerLeft) && UpperRight.Equals(other.UpperRight);

        public override bool Equals(object? obj) => obj is LeveledRectangle other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (LowerLeft.GetHashCode() * 397) ^ UpperRight.GetHashCode();
            }
        }

        public bool ContainsPoint(Point2D point) =>
            LowerLeft.X.IsLessThanOrApproximatelyEqualTo(point.X) &&
            LowerLeft.Y.IsLessThanOrApproximatelyEqualTo(point.Y) &&
            UpperRight.X.IsGreaterThanOrApproximatelyEqualTo(point.X) &&
            UpperRight.Y.IsGreaterThanOrApproximatelyEqualTo(point.Y);

        public LeveledRectangle Extend(LeveledRectangle other)
        {
            var minX = Math.Min(LowerLeft.X, other.LowerLeft.X);
            var minY = Math.Min(LowerLeft.Y, other.LowerLeft.Y);
            var maxX = Math.Max(UpperRight.X, other.UpperRight.X);
            var maxY = Math.Max(UpperRight.Y, UpperRight.Y);
            return new LeveledRectangle(new Point2D(minX, minY), new Point2D(maxX, maxY));
        }

        public static bool operator ==(LeveledRectangle left, LeveledRectangle right) => left.Equals(right);

        public static bool operator !=(LeveledRectangle left, LeveledRectangle right) => !left.Equals(right);
    }
}