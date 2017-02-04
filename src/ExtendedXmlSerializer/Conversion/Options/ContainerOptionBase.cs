using System.Reflection;
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Options
{
	abstract class ContainerOptionBase : ConverterOptionBase
	{
		readonly IStartElementProvider _provider;

		protected ContainerOptionBase(ISpecification<TypeInfo> specification, IStartElementProvider provider)
			: base(specification)
		{
			_provider = provider;
		}

		public override IConverter Get(TypeInfo parameter) => Create(parameter, _provider.Get(parameter));

		protected abstract IConverter Create(TypeInfo type, IEmitter start);
	}
}