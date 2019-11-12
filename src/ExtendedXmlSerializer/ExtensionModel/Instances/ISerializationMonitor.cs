using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	public interface ISerializationMonitor : ISerializationMonitor<object> {}

	/// <summary>
	/// Provides a mechanism to listen in to important events during the serialization process.
	/// </summary>
	/// <typeparam name="T">The type to monitor.</typeparam>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/264#issuecomment-544104668"/>
	public interface ISerializationMonitor<in T>
	{
		void OnSerializing(IFormatWriter writer, T instance);

		void OnSerialized(IFormatWriter writer, T instance);

		void OnDeserializing(IFormatReader reader, Type instanceType);

		void OnActivating(IFormatReader reader, Type instanceType);

		void OnActivated(T instance);

		void OnDeserialized(IFormatReader reader, T instance);
	}
}