using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public class GenericNameOption : NameOptionBase
	{
		public GenericNameOption(INames names) : base(IsGenericTypeSpecification.Default, new GenericNameProvider(names).Get) {}
	}
}