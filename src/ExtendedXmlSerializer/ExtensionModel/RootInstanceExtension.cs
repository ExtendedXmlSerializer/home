using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.References;
using JetBrains.Annotations;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class RootInstanceExtension : ISerializerExtension
	{
		public static RootInstanceExtension Default { get; } = new RootInstanceExtension();

		RootInstanceExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<IRootInstances>(RootInstances.Default)
			            .Decorate<ISerializers, Serializers>();

		sealed class Serializers : ISerializers
		{
			readonly ISpecification<object>   _conditions;
			readonly ISpecification<TypeInfo> _specification;
			readonly ISerializers             _writers;
			readonly IRootInstances           _instances;

			[UsedImplicitly]
			public Serializers(ISerializers writers, IRootInstances instances)
				: this(new InstanceConditionalSpecification(), IsReferenceSpecification.Default, writers, instances) {}

			// ReSharper disable once TooManyDependencies
			public Serializers(ISpecification<object> conditions, ISpecification<TypeInfo> specification,
			                   ISerializers writers, IRootInstances instances)
			{
				_conditions    = conditions;
				_specification = specification;
				_writers       = writers;
				_instances     = instances;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var write = _writers.Get(parameter);
				var result = _specification.IsSatisfiedBy(parameter)
					             ? new Serializer(_conditions, _instances, write)
					             : write;
				return result;
			}
		}

		sealed class Serializer : ISerializer
		{
			readonly ISpecification<object> _conditions;
			readonly IRootInstances         _instances;
			readonly ISerializer            _container;

			public Serializer(ISpecification<object> conditions, IRootInstances instances, ISerializer container)
			{
				_conditions = conditions;
				_instances  = instances;
				_container  = container;
			}

			public object Get(IFormatReader parameter) => _container.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				var item = writer.Get();
				if (_conditions.IsSatisfiedBy(item))
				{
					_instances.Assign(item, instance);
				}

				_container.Write(writer, instance);
			}
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}