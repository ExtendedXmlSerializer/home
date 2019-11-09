using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class RegisteredContents : IContents
	{
		readonly IActivators              _activators;
		readonly ITypedTable<ISerializer> _serializers;

		public RegisteredContents(IActivators activators, ICustomSerializers serializers)
		{
			_activators  = activators;
			_serializers = serializers;
		}

		public ISerializer Get(TypeInfo parameter) => _serializers.Get(parameter) ?? Activate(parameter);

		ISerializer Activate(TypeInfo parameter)
		{
			var typeInfo = parameter.GetCustomAttribute<ContentSerializerAttribute>()
			                        .SerializerType.GetTypeInfo();
			var instance = _activators.Get(typeInfo)
			                          .Get();
			var result = instance as ISerializer ?? GenericSerializers.Default.Get(parameter)
			                                                          .Invoke(instance);
			return result;
		}
	}
}