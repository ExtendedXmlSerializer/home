using ExtendedXmlSerializer.ContentModel.Format;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Instances
{
	/// <summary>
	/// A generalized serialization interceptor base class for convenience.
	/// </summary>
	public abstract class SerializationActivator : ISerializationInterceptor
	{
		/// <inheritdoc />
		public virtual object Serializing(IFormatWriter writer, object instance) => instance;

		/// <inheritdoc />
		public abstract object Activating(Type instanceType);

		/// <inheritdoc />
		public virtual object Deserialized(IFormatReader reader, object instance) => instance;
	}
}