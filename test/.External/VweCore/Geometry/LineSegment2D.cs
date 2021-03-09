using System;

namespace VweCore.Geometry
{
    public readonly struct LineSegment2D : IEquatable<LineSegment2D>
    {
        public LineSegment2D(Point2D point1, Point2D point2)
        {
            Point1 = point1;
            Point2 = point2;
            LineEquation = LineEquation.FromPoints(point1, point2);
        }

        public Point2D Point1 { get; }

        public Point2D Point2 { get; }

        public LineEquation LineEquation { get; }

        public bool Equals(LineSegment2D other) =>
            Point1.Equals(other.Point1) && Point2.Equals(other.Point2);

        public override bool Equals(object? obj) =>
            obj is LineSegment2D other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Point1.GetHashCode() * 397) ^ Point2.GetHashCode();
            }
        }

        public static bool operator ==(LineSegment2D left, LineSegment2D right) => left.Equals(right);

        public static bool operator !=(LineSegment2D left, LineSegment2D right) => !left.Equals(right);

        public bool IsPointOnLineSegment(Point2D point)
        {
            if (!LineEquation.ContainsPoint(point))
                return false;

            var leveledRectangle = new LeveledRectangle(Point1, Point2);
            return leveledRectangle.ContainsPoint(point);
        }

        public double CalculateLength() => Point1.CalculateMagnitude(Point2);

        public LineSegment2D RotateAroundReferencePoint(double angleInDegrees, Point2D referencePoint)
        {
            var newPoint1 = Point1.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            var newPoint2 = Point2.RotateAroundReferencePoint(angleInDegrees, referencePoint);
            return new LineSegment2D(newPoint1, newPoint2);
        }

        public LineSegment2D RotateAroundOrigin(double angleInDegrees)
        {
            var newPoint1 = Point1.RotateAroundOrigin(angleInDegrees);
            var newPoint2 = Point2.RotateAroundOrigin(angleInDegrees);
            return new LineSegment2D(newPoint1, newPoint2);
        }

        public Point2D GetPointByOffset(double offset, bool isFromPoint1)
        {
            Point2D from = isFromPoint1 ? Point1 : Point2;
            Point2D to = isFromPoint1 ? Point2 : Point1;

            AbscissaDirection abscissaDirection = from.X.IsLessThanOrApproximatelyEqualTo(to.X) ? AbscissaDirection.LeftToRight : AbscissaDirection.RightToLeft;
            OrdinateDirection ordinateDirection = from.Y.IsLessThanOrApproximatelyEqualTo(to.Y) ? OrdinateDirection.BottomToTop : OrdinateDirection.TopToBottom;

            var point = LineEquation.GetPointByOffset(from, offset, abscissaDirection, ordinateDirection);

            if (point is null)
                throw new InvalidOperationException($"Cannot get point on line segment {this} by offset {offset}");

            return point.Value;
        }

        public override string ToString() => $"{Point1} to {Point2}";
    }
}