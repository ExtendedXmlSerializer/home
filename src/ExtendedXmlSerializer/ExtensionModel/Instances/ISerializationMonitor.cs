using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	/// <summary>
	/// Provides a generalized mechanism to listen in to important events during the serialization process.
	/// </summary>
	public interface ISerializationMonitor : ISerializationMonitor<object> {}

	/// <summary>
	/// Provides a mechanism to listen in to important events during the serialization process.
	/// </summary>
	/// <typeparam name="T">The type to monitor.</typeparam>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/264#issuecomment-544104668"/>
	public interface ISerializationMonitor<in T>
	{
		/// <summary>
		/// Called when an instance is about to be serialized.
		/// </summary>
		/// <param name="writer">The writer performing the writing to the destination.</param>
		/// <param name="instance">The instance about to be serialized.</param>
		void OnSerializing(IFormatWriter writer, T instance);

		/// <summary>
		/// Called when an instance is serialized.
		/// </summary>
		/// <param name="writer">The writer performing the writing to the destination.</param>
		/// <param name="instance">The instance that was serialized.</param>
		void OnSerialized(IFormatWriter writer, T instance);

		/// <summary>
		/// Called when a reader is about to deserialize an identified type.
		/// </summary>
		/// <param name="reader">The reader representing the source document.</param>
		/// <param name="instanceType">The identified type about to be deserialized.</param>
		void OnDeserializing(IFormatReader reader, Type instanceType);

		/// <summary>
		/// Called when a particular type is about to be activated/instantiated.
		/// </summary>
		/// <param name="reader">The reader representing the source document.</param>
		/// <param name="instanceType">The identified type about to be activated.</param>
		void OnActivating(IFormatReader reader, Type instanceType);

		/// <summary>
		/// Called when an instance of the monitored type is activated/instantiated.
		/// </summary>
		/// <param name="instance">Instance that was activated.</param>
		void OnActivated(T instance);

		/// <summary>
		/// Called when an instance has been fully deserialized.
		/// </summary>
		/// <param name="reader">The reader representing the source document.</param>
		/// <param name="instance">The instance that was deserialized.</param>
		void OnDeserialized(IFormatReader reader, T instance);
	}
}