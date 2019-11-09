using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ConditionalElementDecoration<T> : DecorateAlteration<IElement, T, TypeInfo, IWriter> where T : IElement
	{
		public ConditionalElementDecoration(ISpecification<TypeInfo> specification)
			: base(new Factory(specification).Create) {}

		sealed class Factory
		{
			readonly ISpecification<TypeInfo> _specification;

			public Factory(ISpecification<TypeInfo> specification) => _specification = specification;

			public IElement Create(IElement element, T arg2) => new ConditionalElement(_specification, arg2, element);
		}
	}
}