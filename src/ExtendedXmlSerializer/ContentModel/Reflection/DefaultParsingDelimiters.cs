using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class DefaultParsingDelimiters
	{
		public static DefaultParsingDelimiters Default { get; } = new DefaultParsingDelimiters();

		DefaultParsingDelimiters() : this(new Delimiter('-')) {}

		public DefaultParsingDelimiters(Delimiter nestedClass)
		{
			NestedClass = nestedClass;
		}

		public Delimiter NestedClass { get; }
	}
}