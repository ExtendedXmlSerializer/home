using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	public interface ISerializationInterceptor : ISerializationInterceptor<object> {}

	public interface ISerializationInterceptor<T>
	{
		T Serializing(IFormatWriter writer, T instance);

		T Activating(Type instanceType);

		T Deserialized(IFormatReader reader, T instance);
	}
}
