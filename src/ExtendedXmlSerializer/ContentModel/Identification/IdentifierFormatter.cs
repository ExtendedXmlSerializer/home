namespace ExtendedXmlSerializer.ContentModel.Identification
{
	sealed class IdentifierFormatter : IIdentifierFormatter
	{
		public static IdentifierFormatter Default { get; } = new IdentifierFormatter();

		IdentifierFormatter() {}

		public string Get(int parameter)
		{
			var count  = parameter.ToString();
			var result = $"ns{count}";
			return result;
		}
	}
}