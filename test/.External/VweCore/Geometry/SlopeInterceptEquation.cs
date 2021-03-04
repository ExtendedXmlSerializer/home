using System;

namespace VweCore.Geometry
{
    public readonly struct SlopeInterceptEquation : IEquatable<SlopeInterceptEquation>
    {
        public SlopeInterceptEquation(double slope, double yIntercept)
        {
            Slope = slope;
            YIntercept = yIntercept;
        }

        public double Slope { get; }

        public double YIntercept { get; }

        public bool Equals(SlopeInterceptEquation other) => Slope.IsApproximately(other.Slope) && YIntercept.IsApproximately(other.YIntercept);

        public override bool Equals(object? obj) => obj is SlopeInterceptEquation other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Slope.GetHashCode() * 397) ^ YIntercept.GetHashCode();
            }
        }

        public static bool operator ==(SlopeInterceptEquation left, SlopeInterceptEquation right) => left.Equals(right);

        public static bool operator !=(SlopeInterceptEquation left, SlopeInterceptEquation right) => !left.Equals(right);

        public double CalculateY(double x) => (x * Slope) + YIntercept;

        public bool ContainsPoint(Point2D point) => CalculateY(point.X).IsApproximately(point.Y);

        public Point2D? CalculateIntersection(SlopeInterceptEquation otherEquation)
        {
            if (Slope.IsApproximately(otherEquation.Slope))
                return null;

            var slopeDifference = Slope - otherEquation.Slope;
            var yIntersectionDifference = otherEquation.YIntercept - YIntercept;
            var x = yIntersectionDifference / slopeDifference;
            var y = CalculateY(x);
            return new Point2D(x, y);
        }

        public Point2D? CalculateIntersection(LineEquation otherEquation)
        {
            if (otherEquation.SlopeInterceptEquation.HasValue)
                return CalculateIntersection(otherEquation.SlopeInterceptEquation.Value);

            var x = otherEquation.X!.Value;
            var y = CalculateY(x);
            return new Point2D(x, y);
        }

        public Point2D GetPointByDistance(double x, double distance, AbscissaDirection direction)
        {
            if (distance.IsApproximately(0.0))
                return new Point2D(x, CalculateY(x));

            if (distance.IsLessThanOrApproximatelyEqualTo(0.0))
            {
                distance = Math.Abs(distance);
                direction = direction == AbscissaDirection.LeftToRight ? AbscissaDirection.RightToLeft : AbscissaDirection.LeftToRight;
            }

            var xDistance = Math.Sqrt(distance.Square() / (Slope.Square() + 1));

            var x1 = x - xDistance;
            var x2 = x + xDistance;

            var pointX = direction == AbscissaDirection.LeftToRight
                ? (x1.IsGreaterThanOrApproximatelyEqualTo(x) ? x1 : x2)
                : (x1.IsLessThanOrApproximatelyEqualTo(x) ? x1 : x2);

            return new Point2D(pointX, CalculateY(pointX));
        }

        public Point2D CalculateIntersection(double x)
        {
            var y = CalculateY(x);
            return new Point2D(x, y);
        }

        public override string ToString()
        {
            var result = "y = " + Slope.ToInvariantString() + "x";
            if (YIntercept.IsApproximately(0))
                return result;

            if (YIntercept < 0)
                result += " - " + (-YIntercept).ToInvariantString();
            else
                result += " + " + YIntercept.ToInvariantString();

            return result;
        }
    }
}