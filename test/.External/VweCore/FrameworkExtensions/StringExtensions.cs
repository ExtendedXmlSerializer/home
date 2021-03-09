using System.Globalization;
using Light.GuardClauses;

namespace VweCore.FrameworkExtensions
{
    public static class StringExtensions
    {
        private static readonly CultureInfo GermanCulture = CultureInfo.CreateSpecificCulture("de-DE");
        private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Reads a string until a specific character is found.
        /// </summary>
        /// <param name="text">String containing the substring.</param>
        /// <param name="startIndex">Start Index of the substring.</param>
        /// <param name="stopAt">When this character is found returns the substring.</param>
        /// <returns>
        /// String containing either the original string when stopAt character was not found or
        /// a substring from startIndex to stopAt character.
        /// </returns>
        public static string ReadUntil(this string text, int startIndex, char stopAt)
        {
            if (text.IsNullOrWhiteSpace() || text.Length <= startIndex)
                return text;

            var foundLength = 0;
            for (var i = startIndex; i < text.Length; ++i)
            {
                if (text[i] == stopAt)
                    break;

                ++foundLength;
            }

            return text.Substring(startIndex, foundLength);
        }

        public static bool TryParseToDouble(this string? value, out double number)
        {
            if (value.IsNullOrWhiteSpace())
                goto CouldNotParse;

            var numberOfCommas = DetermineNumberOfCommas(value);
            var cultureInfo = InvariantCulture;
            if (numberOfCommas == 1)
                cultureInfo = GermanCulture;

            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, out number))
                return true;

            CouldNotParse:
            number = default;
            return false;
        }

        private static int DetermineNumberOfCommas(string text)
        {
            var numberOfCommas = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == ',')
                    ++numberOfCommas;
            }

            return numberOfCommas;
        }
    }
}