using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Options
{
	abstract class ContainerOptionBase : ConverterOptionBase
	{
		readonly IElementProvider _element;

		protected ContainerOptionBase(ISpecification<TypeInfo> specification, IElementProvider element)
			: base(specification)
		{
			_element = element;
		}

		public override IConverter Get(TypeInfo parameter) => Create(parameter, _element.Get(parameter));

		protected abstract IConverter Create(TypeInfo type, IElement name);
	}
}