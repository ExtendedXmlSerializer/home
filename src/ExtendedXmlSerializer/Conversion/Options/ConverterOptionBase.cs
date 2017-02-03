using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Options
{
	public abstract class ConverterOptionBase : OptionBase<TypeInfo, IConverter>, IConverterOption
	{
		protected ConverterOptionBase(ISpecification<TypeInfo> specification) : base(specification) {}
	}
}