namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class Identifier : Core.Parsing.Identifier
	{
		public static Identifier Default { get; } = new Identifier();

		Identifier() : base('-', '_') {}
	}
}