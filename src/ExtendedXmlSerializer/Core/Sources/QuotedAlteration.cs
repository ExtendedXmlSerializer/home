namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class QuotedAlteration : IAlteration<string>
	{
		public static QuotedAlteration Default { get; } = new QuotedAlteration();

		QuotedAlteration() {}

		public string Get(string parameter) => $@"""{parameter}""";
	}
}