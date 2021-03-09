using System;

namespace VweCore.Geometry
{
    public readonly struct Ray2D : IEquatable<Ray2D>
    {
        public Ray2D(Point2D referencePoint, double angleInDegrees)
        {
            ReferencePoint = referencePoint;
            AngleInDegrees = angleInDegrees;
            var direction = new Point2D(referencePoint.X + 1, referencePoint.Y).RotateAroundReferencePoint(angleInDegrees, referencePoint);
            LineEquation = LineEquation.FromPoints(ReferencePoint, direction);
        }

        public Point2D ReferencePoint { get; }

        public double AngleInDegrees { get; }

        public LineEquation LineEquation { get; }

        public bool IsPointOnRay(Point2D point)
        {
            if (!LineEquation.ContainsPoint(point))
                return false;

            var normalizedAngle = AngleInDegrees.NormalizeAngleInDegrees();

            if (normalizedAngle.IsGreaterThanOrApproximatelyEqualTo(0) && normalizedAngle < 90)
                return point.X.IsGreaterThanOrApproximatelyEqualTo(ReferencePoint.X);
            if (normalizedAngle.IsGreaterThanOrApproximatelyEqualTo(90) && normalizedAngle < 180)
                return point.Y.IsGreaterThanOrApproximatelyEqualTo(ReferencePoint.Y);
            if (normalizedAngle.IsGreaterThanOrApproximatelyEqualTo(180) && normalizedAngle < 270)
                return point.X.IsLessThanOrApproximatelyEqualTo(ReferencePoint.X);

            return point.Y.IsLessThanOrApproximatelyEqualTo(ReferencePoint.Y);
        }

        public bool Equals(Ray2D other) => ReferencePoint.Equals(other.ReferencePoint) && AngleInDegrees.Equals(other.AngleInDegrees);

        public override bool Equals(object? obj) => obj is Ray2D other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (ReferencePoint.GetHashCode() * 397) ^ AngleInDegrees.GetHashCode();
            }
        }

        public static bool operator ==(Ray2D left, Ray2D right) => left.Equals(right);

        public static bool operator !=(Ray2D left, Ray2D right) => !left.Equals(right);

        public override string ToString() => $"{ReferencePoint} {AngleInDegrees.ToInvariantString()}°";
    }
}