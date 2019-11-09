namespace ExtendedXmlSerializer.Core
{
	sealed class DefaultClrDelimiters
	{
		public static DefaultClrDelimiters Default { get; } = new DefaultClrDelimiters();

		DefaultClrDelimiters() : this(new Delimiter('+'), new Delimiter('`'), new Delimiter('.')) {}

		public DefaultClrDelimiters(Delimiter nestedClass, Delimiter generic, Delimiter separator)
		{
			NestedClass = nestedClass;
			Generic     = generic;
			Separator   = separator;
		}

		public Delimiter NestedClass { get; }
		public Delimiter Generic { get; }
		public Delimiter Separator { get; }
	}
}