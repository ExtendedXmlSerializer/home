using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class GenericStartOption : StartOptionBase
	{
		public GenericStartOption(IStartElementProvider provider) : base(IsGenericTypeSpecification.Default, provider.Get) {}
	}
}