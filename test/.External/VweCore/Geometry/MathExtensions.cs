using System;

namespace VweCore.Geometry
{
    public static class MathExtensions
    {
        public static double ToRadians(this double angleInDegrees) => angleInDegrees * Math.PI / 180.0;

        public static double ToDegrees(this double angleInRadians) => angleInRadians * 180 / Math.PI;

        public static double Square(this double value) => value * value;

        public static double CalculateMagnitude(this Point2D point) => Math.Sqrt(point.X.Square() + point.Y.Square());

        public static double CalculateMagnitude(this Point2D point, Point2D other)
        {
            var (xDistance, yDistance) = point.CalculateDistances(other);
            return Math.Sqrt(xDistance.Square() + yDistance.Square());
        }

        public static bool TryCalculateSlope(this Point2D point, Point2D other, out double slope)
        {
            var (xDistance, yDistance) = other.CalculateDistances(point);
            if (xDistance.IsApproximately(0))
            {
                slope = default;
                return false;
            }

            slope = yDistance / xDistance;
            return true;
        }

        public static double CalculateYIntercept(this Point2D point, double slope) => point.Y - slope * point.X;

        public static (double xDistance, double yDistance) CalculateDistances(this Point2D point, Point2D other) => (point.X - other.X, point.Y - other.Y);

        public static double NormalizeAngleInDegrees(this double angleInDegrees)
        {
            if (angleInDegrees < 0.0)
                return 360.0 + angleInDegrees % -360.0;
            if (angleInDegrees < 360.0)
                return angleInDegrees;

            return angleInDegrees % 360.0;
        }

        public static double CalculateDotProduct(this Point2D point, Point2D other) => point.X * other.X + point.Y * other.Y;

        public static double CalculateDirectionAngle(this Point2D vector) =>
            Math.Atan2(vector.Y, vector.X).ToDegrees();

        public static double CalculateAngleInDegreesBetweenVectors(this Point2D vector, Point2D otherVector)
        {
            vector.MustNotBeZeroVector(nameof(vector));
            otherVector.MustNotBeZeroVector(nameof(otherVector));

            if (vector.Equals(otherVector))
                return 0.0;

            var numerator = vector.CalculateDotProduct(otherVector);
            var denominator = vector.CalculateMagnitude() * otherVector.CalculateMagnitude();
            var angleInRadians = Math.Acos(numerator / denominator);
            return angleInRadians.ToDegrees();
        }

        public static Point2D MustNotBeZeroVector(this Point2D vector, string? parameterName = null)
        {
            if (vector.Equals(Point2D.Zero))
                throw new ArgumentException($"{parameterName ?? "The specified vector"} must not be the zero vector, but it is.", parameterName);
            return vector;
        }
    }
}