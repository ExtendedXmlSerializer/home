using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	public sealed class AllowedInstancesExtension
		: TypedTable<ISpecification<object>>, ITypedSpecifications, ISerializerExtension
	{
		public AllowedInstancesExtension() :
			base(new Dictionary<TypeInfo, ISpecification<object>>(ReflectionModel.Defaults.TypeComparer)) {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<ITypedSpecifications>(this)
			            .Decorate<ISerializers, Serializers>()
			            .Decorate<IMemberAccessors, MemberAccessors>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Serializers : ISerializers
		{
			readonly IParameterizedSource<TypeInfo, ISpecification<object>> _specifications;
			readonly ISerializers                                           _serializers;

			public Serializers(ITypedSpecifications specifications, ISerializers serializers)
			{
				_specifications = specifications;
				_serializers    = serializers;
			}

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var specification = _specifications.Get(parameter);
				var serializer    = _serializers.Get(parameter);
				var result        = specification != null ? new Serializer(specification, serializer) : serializer;
				return result;
			}
		}

		sealed class MemberAccessors : IMemberAccessors
		{
			readonly ITypedSpecifications _specifications;
			readonly IMemberAccessors     _accessors;

			public MemberAccessors(ITypedSpecifications specifications, IMemberAccessors accessors)
			{
				_specifications = specifications;
				_accessors      = accessors;
			}

			public IMemberAccess Get(IMember parameter)
			{
				var specification = _specifications.Get(parameter.MemberType);
				var access        = _accessors.Get(parameter);
				var result =
					specification != null ? new MemberAccess(specification.And(access), access) : access;
				return result;
			}

			sealed class MemberAccess : IMemberAccess
			{
				readonly ISpecification<object> _specification;
				readonly IMemberAccess          _access;

				public MemberAccess(ISpecification<object> specification, IMemberAccess access)
				{
					_specification = specification;
					_access        = access;
				}

				public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);

				public ISpecification<object> Instance => _access.Instance;

				public object Get(object instance) => _access.Get(instance);

				public void Assign(object instance, object value)
				{
					_access.Assign(instance, value);
				}
			}
		}

		sealed class Serializer : ContentModel.ISerializer
		{
			readonly ISpecification<object> _specification;
			readonly ContentModel.ISerializer            _serializer;

			public Serializer(ISpecification<object> specification, ContentModel.ISerializer serializer)
			{
				_specification = specification;
				_serializer    = serializer;
			}

			public object Get(IFormatReader parameter) => _serializer.Get(parameter);

			public void Write(IFormatWriter writer, object instance)
			{
				if (_specification.IsSatisfiedBy(instance))
				{
					_serializer.Write(writer, instance);
				}
			}
		}
	}
}