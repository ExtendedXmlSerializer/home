using System;
using System.Globalization;

namespace VweCore.Geometry
{
    public static class Tolerance
    {
        public const double Value = 0.001;

        public static bool IsGreaterThanOrApproximatelyEqualTo(this double value, double other) =>
            value > other || value.IsApproximately(other);

        public static bool IsLessThanOrApproximatelyEqualTo(this double value, double other) =>
            value < other || value.IsApproximately(other);

        public static bool IsApproximately(this double value, double other)
        {
            value = value.Round();
            other = other.Round();
            var distance = Math.Abs(value - other);
            var roundedDistance = distance.Round();
            return roundedDistance < Value;
        }

        public static bool IsApproximately(this double? nullableValue, double? other) =>
            nullableValue == null ? other == null : other != null && nullableValue.Value.IsApproximately(other.Value);

        public static double Round(this double value) => Math.Round(value, 3, MidpointRounding.AwayFromZero);

        public static string ToInvariantString(this double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}