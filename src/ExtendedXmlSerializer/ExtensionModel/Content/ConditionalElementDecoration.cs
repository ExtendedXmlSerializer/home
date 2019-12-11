using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Linq;
using System.Reflection;

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

	sealed class ConditionalElementDecoration<TSpecification, T> : IAlteration<IServiceRepository>
		where T : IElement where TSpecification : ISpecification<TypeInfo>
	{
		public static ConditionalElementDecoration<TSpecification, T> Default { get; } =
			new ConditionalElementDecoration<TSpecification, T>();

		ConditionalElementDecoration() {}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var key = Support<TSpecification>.Metadata;
			var repository = key.IsClass &&
			                 !parameter.AvailableServices.Contains(key)
				                 ? parameter.RegisterWithDependencies<TSpecification>()
				                 : parameter;
			return repository.RegisterWithDependencies<T>()
			                 .Decorate<IElement>(Create);
		}

		static IElement Create(IServiceProvider arg1, IElement arg2)
			=> new ConditionalElement(arg1.Get<TSpecification>(), arg1.Get<T>(), arg2);
	}
}