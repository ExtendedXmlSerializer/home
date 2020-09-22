using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	sealed class SerializationInterceptor<T> : ISerializationInterceptor
	{
		readonly ISerializationInterceptor<T> _interceptor;

		public SerializationInterceptor(ISerializationInterceptor<T> interceptor) => _interceptor = interceptor;

		public object Serializing(IFormatWriter writer, object instance)
			=> _interceptor.Serializing(writer, (T)instance);

		public object Activating(Type instanceType) => _interceptor.Activating(instanceType);

		public object Deserialized(IFormatReader reader, object instance)
			=> _interceptor.Deserialized(reader, (T)instance);
	}
}