using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class GenericElementOption : ElementOptionBase
	{
		public GenericElementOption(IElementProvider provider) : base(IsGenericTypeSpecification.Default, provider.Get) {}
	}
}