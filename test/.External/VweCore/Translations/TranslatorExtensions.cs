using System.Globalization;
using Light.GuardClauses;

namespace VweCore.Translations
{
    public static class TranslatorExtensions
    {
        public static string TranslateAndFormat(this ITranslator service, string key, params object?[] parameters)
        {
            var messageTemplate = service.MustNotBeNull(nameof(service)).GetTranslation(key);
            return string.Format(CultureInfo.CurrentCulture, messageTemplate, parameters);
        }
    }
}