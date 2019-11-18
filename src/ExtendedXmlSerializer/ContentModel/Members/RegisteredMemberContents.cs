using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RegisteredMemberContents : IMemberContents, ISpecification<IMember>
	{
		readonly IActivators                                   _activators;
		readonly ISpecification<MemberInfo>                    _specification;
		readonly IParameterizedSource<MemberInfo, ISerializer> _serializers;

		public RegisteredMemberContents(IActivators activators, ICustomMemberSerializers serializers)
			: this(activators, serializers.Or(IsDefinedSpecification<ContentSerializerAttribute>.Default),
			       serializers) {}

		public RegisteredMemberContents(IActivators activators, ISpecification<MemberInfo> specification,
		                                IParameterizedSource<MemberInfo, ISerializer> serializers)
		{
			_activators    = activators;
			_specification = specification;
			_serializers   = serializers;
		}

		public bool IsSatisfiedBy(IMember parameter) => _specification.IsSatisfiedBy(parameter.Metadata);

		public ISerializer Get(IMember parameter) => _serializers.Get(parameter.Metadata) ?? Activate(parameter);

		ISerializer Activate(IMember parameter)
		{
			var type     = parameter.Metadata.GetCustomAttribute<ContentSerializerAttribute>().SerializerType;
			var instance = _activators.Get(type).Get();
			var result = instance as ISerializer ?? GenericSerializers.Default.Get(parameter.MemberType)
			                                                          .Invoke(instance);
			return result;
		}
	}
}