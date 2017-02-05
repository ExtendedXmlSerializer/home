using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ConverterModel
{
	public interface IParsingDelimiters
	{
		Delimiter Assembly { get; }
		Delimiter Namespace { get; }
		Delimiter NestedClass { get; }
		Delimiter Part { get; }
	}
}