using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public class GenericElementOption : ElementOptionBase
	{
		public GenericElementOption(IElements elements) : base(IsGenericTypeSpecification.Default, new GenericElementProvider(elements).Get) {}
	}
}