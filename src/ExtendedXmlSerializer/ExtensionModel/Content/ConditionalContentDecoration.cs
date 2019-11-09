using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ConditionalContentDecoration<TSpecification, T> : IAlteration<IServiceRepository>
		where T : IContents where TSpecification : ISpecification<TypeInfo>
	{
		public static ConditionalContentDecoration<TSpecification, T> Default { get; } =
			new ConditionalContentDecoration<TSpecification, T>();

		ConditionalContentDecoration() {}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var key = Support<TSpecification>.Key;
			var repository = key.IsClass &&
			                 !parameter.AvailableServices.Contains(key)
				                 ? parameter.RegisterWithDependencies<TSpecification>()
				                 : parameter;
			return repository.RegisterWithDependencies<T>()
			                 .Decorate<IContents>(Create);
		}

		static IContents Create(IServiceProvider arg1, IContents arg2)
			=> new ConditionalContents(arg1.Get<TSpecification>(), arg1.Get<T>(), arg2);
	}

	sealed class ConditionalContentDecoration<T> : DecorateAlteration<IContents, T, TypeInfo, ISerializer>
		where T : IContents
	{
		public ConditionalContentDecoration(ISpecification<TypeInfo> specification)
			: base(new Factory(specification).Create) {}

		sealed class Factory
		{
			readonly ISpecification<TypeInfo> _specification;

			public Factory(ISpecification<TypeInfo> specification) => _specification = specification;

			public IContents Create(IContents element, T arg2)
				=> new ConditionalContents(_specification, arg2, element);
		}
	}
}