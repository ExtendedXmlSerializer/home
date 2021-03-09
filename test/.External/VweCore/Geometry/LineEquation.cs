using System;

namespace VweCore.Geometry
{
    public readonly struct LineEquation : IEquatable<LineEquation>
    {
        public LineEquation(SlopeInterceptEquation slopeInterceptEquation)
        {
            SlopeInterceptEquation = slopeInterceptEquation;
            X = null;
        }

        public LineEquation(double x)
        {
            X = x;
            SlopeInterceptEquation = null;
        }

        public SlopeInterceptEquation? SlopeInterceptEquation { get; }

        public double? X { get; }

        public static LineEquation FromPoints(Point2D point1, Point2D point2)
        {
            if (!point1.TryCalculateSlope(point2, out var slope))
                return new LineEquation(point1.X);

            var yIntercept = point1.CalculateYIntercept(slope);
            return new LineEquation(new SlopeInterceptEquation(slope, yIntercept));
        }

        public double CalculateYValue(double x) => SlopeInterceptEquation?.CalculateY(x) ?? X!.Value;

        public bool ContainsPoint(Point2D point) => SlopeInterceptEquation?.ContainsPoint(point) ?? point.X.IsApproximately(X!.Value);

        public Point2D? CalculateIntersection(LineEquation otherEquation)
        {
            if (SlopeInterceptEquation.HasValue)
                return SlopeInterceptEquation.Value.CalculateIntersection(otherEquation);

            if (otherEquation.X.HasValue)
                return null;

            return otherEquation.SlopeInterceptEquation!.Value.CalculateIntersection(X!.Value);
        }

        public Point2D? GetPointByOffset(Point2D pointFrom, double offset, AbscissaDirection abscissaDirection, OrdinateDirection ordinateDirection)
        {
            if (!ContainsPoint(pointFrom))
                return null;

            if (offset.IsLessThanOrApproximatelyEqualTo(0.0))
            {
                offset = Math.Abs(offset);
                abscissaDirection = abscissaDirection == AbscissaDirection.LeftToRight ? AbscissaDirection.RightToLeft : AbscissaDirection.LeftToRight;
                ordinateDirection = ordinateDirection == OrdinateDirection.BottomToTop ? OrdinateDirection.TopToBottom : OrdinateDirection.BottomToTop;
            }

            return SlopeInterceptEquation?.GetPointByDistance(pointFrom.X, offset, abscissaDirection) ?? new Point2D(pointFrom.X, ordinateDirection == OrdinateDirection.BottomToTop ? pointFrom.Y + offset : pointFrom.Y - offset);
        }

        public bool Equals(LineEquation other) => Nullable.Equals(SlopeInterceptEquation, other.SlopeInterceptEquation) && Nullable.Equals(X, other.X);

        public override bool Equals(object? obj) => obj is LineEquation other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (SlopeInterceptEquation.GetHashCode() * 397) ^ X.GetHashCode();
            }
        }

        public static bool operator ==(LineEquation left, LineEquation right) => left.Equals(right);

        public static bool operator !=(LineEquation left, LineEquation right) => !left.Equals(right);
    }
}