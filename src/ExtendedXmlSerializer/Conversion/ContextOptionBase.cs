using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
	public abstract class ContextOptionBase : OptionBase<TypeInfo, IElementContext>, IContextOption
	{
		protected ContextOptionBase(ISpecification<TypeInfo> specification) : base(specification) {}
	}
}