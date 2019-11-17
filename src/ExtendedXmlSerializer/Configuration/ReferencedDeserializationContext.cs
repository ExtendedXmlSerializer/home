using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.IO;
using System.Text;
using System.Xml;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Establishes a context for referenced (or targeted) deserialization whereby an existing instance is provided to
	/// assign values into, rather than the default behavior of activating a new instance where the values are assigned.
	/// </summary>
	/// <typeparam name="T">The type under configuration.</typeparam>
	/// <seealso cref="ExtensionMethodsForExtensionModel.UsingTarget{T}"/>
	public sealed class ReferencedDeserializationContext<T> where T : class
	{
		readonly IInstanceReader   _reader;
		readonly T                 _existing;
		readonly XmlReaderSettings _settings;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="this">The serializer to configure.</param>
		/// <param name="existing">The existing target instance.</param>
		public ReferencedDeserializationContext(IExtendedXmlSerializer @this, T existing)
			: this(@this, existing, Defaults.ReaderSettings) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="serializer">The serializer to configure.</param>
		/// <param name="existing">The existing target instance.</param>
		/// <param name="settings">The xml reader settings used for deserialization.</param>
		public ReferencedDeserializationContext(IExtendedXmlSerializer serializer, T existing, XmlReaderSettings settings)
			: this(InstanceReaders.Default.Get(serializer), existing, settings) {}

		ReferencedDeserializationContext(IInstanceReader reader, T existing, XmlReaderSettings settings)
		{
			_reader   = reader;
			_existing = existing;
			_settings = settings;
		}

		/// <summary>
		/// Deserializes a document represented by the provided text and assigns any values into the provided instance context.
		/// </summary>
		/// <param name="data">Text that represents an Xml document.</param>
		/// <returns>The initial provided target instance, assigned with values discovered in the provided document.</returns>
		public T Deserialize(string data) => Deserialize(Defaults.CloseRead, data);

		/// <summary>
		/// Deserializes a document represented by the provided text and assigns any values into the provided instance
		/// context, using the provided reader settings.
		/// </summary>
		/// <param name="settings">The xml reader settings to apply during document processing.</param>
		/// <param name="data">Text that represents an Xml document.</param>
		/// <returns>The initial provided target instance, assigned with values discovered in the provided document.</returns>
		public T Deserialize(XmlReaderSettings settings, string data)
			=> Deserialize(settings, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		/// <summary>
		/// Deserializes a document represented by the provided <paramref name="stream"/> and assigns any values into the provided instance
		/// context.
		/// </summary>
		/// <param name="stream">The stream representing the source document.</param>
		/// <returns>The initial provided target instance, assigned with values discovered in the provided document.</returns>
		public T Deserialize(Stream stream) => Deserialize(_settings, stream);

		/// <summary>
		/// Deserializes a document represented by the provided stream and assigns any values into the provided instance
		/// context, using the provided reader settings.
		/// </summary>
		/// <param name="settings">The xml reader settings to apply during document processing.</param>
		/// <param name="stream">The stream representing the source document.</param>
		/// <returns>The initial provided target instance, assigned with values discovered in the provided document.</returns>
		public T Deserialize(XmlReaderSettings settings, Stream stream)
		{
			var reader   = new XmlReaderFactory(settings, settings.NameTable.Context()).Get(stream);
			var existing = new Existing(reader, _existing);
			var result   = (T)_reader.Get(existing);
			return result;
		}
	}
}