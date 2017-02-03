using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Options
{
	public class FixedConverterOption : FixedOption<TypeInfo, IConverter>, IConverterOption
	{
		public FixedConverterOption(ISpecification<TypeInfo> specification, IConverter context) : base(specification, context) {}
	}
}