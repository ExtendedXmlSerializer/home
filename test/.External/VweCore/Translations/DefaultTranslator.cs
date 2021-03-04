namespace VweCore.Translations
{
    // The default translator is used to provide a default implementation for test scenarios.
    // In production scenarios, the Translator in the WPF project should be used.
    // This implementation simply returns the key.
    public sealed class DefaultTranslator : ITranslator
    {
        public string GetTranslation(string key) => key;
    }
}