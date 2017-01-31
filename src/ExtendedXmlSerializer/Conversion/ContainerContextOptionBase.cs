using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
	abstract class ContainerContextOptionBase<T> : ContextOptionBase where T : IName
	{
		readonly INameProvider<T> _name;

		protected ContainerContextOptionBase(ISpecification<TypeInfo> specification, INameProvider<T> name)
			: base(specification)
		{
			_name = name;
		}

		public override IElementContext Get(TypeInfo parameter) => Create(parameter, _name.Get(parameter));

		protected abstract IElementContext Create(TypeInfo type, T name);
	}
}