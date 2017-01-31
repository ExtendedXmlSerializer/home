using System.Reflection;
using ExtendedXmlSerialization.Conversion.Names;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Options
{
	abstract class ContainerConverterOptionBase<T> : ConverterOptionBase where T : IName
	{
		readonly INameProvider<T> _name;

		protected ContainerConverterOptionBase(ISpecification<TypeInfo> specification, INameProvider<T> name)
			: base(specification)
		{
			_name = name;
		}

		public override IConverter Get(TypeInfo parameter) => Create(parameter, _name.Get(parameter));

		protected abstract IConverter Create(TypeInfo type, T name);
	}
}