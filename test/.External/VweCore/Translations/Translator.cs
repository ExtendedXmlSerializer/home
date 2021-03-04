using Light.GuardClauses;

namespace VweCore.Translations
{
    public static class Translator
    {
        private static ITranslator _instance = new DefaultTranslator();

        public static ITranslator Instance
        {
            get => _instance;
            set => _instance = value.MustNotBeNull();
        }
    }
}