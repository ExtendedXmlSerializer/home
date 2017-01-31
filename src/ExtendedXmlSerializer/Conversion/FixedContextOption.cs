using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
	public class FixedContextOption : FixedOption<TypeInfo, IElementContext>, IContextOption
	{
		public FixedContextOption(ISpecification<TypeInfo> specification, IElementContext context)
			: base(specification, context) {}
	}
}