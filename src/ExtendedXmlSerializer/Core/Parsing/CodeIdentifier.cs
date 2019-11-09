namespace ExtendedXmlSerializer.Core.Parsing
{
	sealed class CodeIdentifier : Identifier
	{
		public static CodeIdentifier Default { get; } = new CodeIdentifier();

		CodeIdentifier() : base("_") {}
	}
}