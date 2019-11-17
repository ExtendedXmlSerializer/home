using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.IO;
using System.Text;
using System.Xml;

// ReSharper disable TooManyArguments

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods for convenience that assist in simplifying the amount of code required for serializing instances
	/// and deserializing document sources.
	/// </summary>
	public static class ExtensionMethodsForSerialization
	{
		readonly static Func<Stream> New = DefaultActivators.Default.New<MemoryStream>;

		readonly static IXmlWriterFactory WriterFactory
			= new XmlWriterFactory(CloseSettings.Default.Get(ExtensionModel.Xml.Defaults.WriterSettings));

		/// <summary>
		/// Serialization convenience method to serialize the provided instance into a string.
		/// </summary>
		/// <param name="this">The serializer to use for serialization.</param>
		/// <param name="instance">The instance to serialize.</param>
		/// <returns>A string that represents the provided instance in Xml format.</returns>
		public static string Serialize(this IExtendedXmlSerializer @this, object instance)
			=> @this.Serialize(WriterFactory, New, instance);

		/// <summary>
		/// Serialization convenience method to serialize the provided instance into a string with the provided
		/// <see cref="XmlWriterSettings"/>.
		/// </summary>
		/// <param name="this">The serializer to use for serialization.</param>
		/// <param name="settings">The writer settings for handling the xml writer used create the resulting Xml.</param>
		/// <param name="instance">The instance to serialize.</param>
		/// <returns>A string that represents the provided instance in Xml format.</returns>
		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, object instance)
			=> @this.Serialize(new XmlWriterFactory(CloseSettings.Default.Get(settings)), New, instance);

		/// <summary>
		/// Serialization convenience method to serialize the provided instance into a string along with the provided
		/// destination <see cref="Stream"/>.
		/// </summary>
		/// <param name="this">The serializer to use for serialization.</param>
		/// <param name="stream">The destination stream.</param>
		/// <param name="instance">The instance to serialize.</param>
		/// <returns>A string that represents the provided instance in Xml format.</returns>
		public static string Serialize(this IExtendedXmlSerializer @this, Stream stream, object instance)
			=> @this.Serialize(XmlWriterFactory.Default, stream.Self, instance);

		/// <summary>
		/// Serialization convenience method to serialize the provided instance into a string along with the provided
		/// destination <see cref="Stream"/> while using the settings configured in the provided
		/// <see cref="XmlWriterSettings"/>.
		/// </summary>
		/// <param name="this">The serializer to use for serialization.</param>
		/// <param name="settings">The writer settings for handling the xml writer used create the resulting Xml.</param>
		/// <param name="stream">The destination stream.</param>
		/// <param name="instance">The instance to serialize.</param>
		/// <returns>A string that represents the provided instance in Xml format.</returns>
		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, Stream stream,
		                               object instance)
			=> @this.Serialize(new XmlWriterFactory(settings), stream.Self, instance);

		static string Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, Func<Stream> stream,
		                        object instance)
			=> new InstanceFormatter(@this, factory, stream).Get(instance);

		/// <summary>
		/// Serialization convenience method to serialize the provided instance into the provided destination
		/// <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="this">The serializer to use for serialization.</param>
		/// <param name="writer">The destination writer.</param>
		/// <param name="instance">The instance to serialize.</param>
		public static void Serialize(this IExtendedXmlSerializer @this, TextWriter writer, object instance)
			=> @this.Serialize(XmlWriterFactory.Default, writer, instance);

		/// <summary>
		/// Serialization convenience method to serialize the provided instance into the provided destination
		/// <see cref="TextWriter"/> while using the settings configured in the provided <see cref="XmlWriterSettings"/>.
		/// </summary>
		/// <param name="this">The serializer to use for serialization.</param>
		/// <param name="settings">The writer settings for handling the xml writer used create the resulting Xml.</param>
		/// <param name="writer">The destination writer.</param>
		/// <param name="instance">The instance to serialize.</param>
		public static void Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, TextWriter writer,
		                             object instance)
			=> @this.Serialize(new XmlWriterFactory(settings), writer, instance);

		static void Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, TextWriter writer,
		                      object instance)
			=> @this.Serialize(factory.Get(writer), instance);

		/// <summary>
		/// Deserialization convenience method to deserialize a document found within the provided string into an instance of
		/// the requested instance type, using reader settings that will close the stream once the process is complete.
		/// </summary>
		/// <typeparam name="T">The requested instance type.</typeparam>
		/// <param name="this">The serializer to create the requested instance.</param>
		/// <param name="data">A text representation of an Xml document.</param>
		/// <returns>An instance of the requested type.</returns>
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, string data)
			=> @this.Deserialize<T>(ExtensionModel.Xml.Defaults.CloseRead, data);

		/// <summary>
		/// Deserialization convenience method to deserialize a document found within the provided string into an instance of
		/// the requested instance type, using the provided <see cref="XmlReaderSettings"/>.
		/// </summary>
		/// <typeparam name="T">The requested instance type.</typeparam>
		/// <param name="this">The serializer to create the requested instance.</param>
		/// <param name="settings">The reader settings for handling the xml reader used create the instance.</param>
		/// <param name="data">A text representation of an Xml document.</param>
		/// <returns>An instance of the requested type.</returns>
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, string data)
			=> @this.Deserialize<T>(settings, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		/// <summary>
		/// Deserialization convenience method to deserialize a document found within the provided <see cref="Stream"/> into
		/// an instance of the requested instance type, using the default reader settings found at
		/// <see cref="ExtensionModel.Xml.Defaults.ReaderSettings"/>.
		/// </summary>
		/// <typeparam name="T">The requested instance type.</typeparam>
		/// <param name="this">The serializer to create the requested instance.</param>
		/// <param name="stream">The stream containing the necessary data to deserialize the object of requested type.</param>
		/// <returns>An instance of the requested type.</returns>
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, Stream stream)
			=> @this.Deserialize<T>(ExtensionModel.Xml.Defaults.ReaderSettings, stream);

		/// <summary>
		/// Deserialization convenience method to deserialize a document found within the provided <see cref="Stream"/> into
		/// an instance of the requested instance type, using the provided <see cref="XmlReaderSettings"/>.
		/// </summary>
		/// <typeparam name="T">The requested instance type.</typeparam>
		/// <param name="this">The serializer to create the requested instance.</param>
		/// <param name="settings">The reader settings for handling the xml reader used create the instance.</param>
		/// <param name="stream">The stream containing the necessary data to deserialize the object of requested type.</param>
		/// <returns>An instance of the requested type.</returns>
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, Stream stream)
			=> @this.Deserialize(new XmlReaderFactory(settings, settings.NameTable.Context()).Get(stream)).AsValid<T>();

		/// <summary>
		/// Deserialization convenience method to deserialize a document found within the provided <see cref="TextReader"/>
		/// into an instance of the requested instance type, using the default reader settings found at
		/// <see cref="ExtensionModel.Xml.Defaults.ReaderSettings"/>.
		/// </summary>
		/// <typeparam name="T">The requested instance type.</typeparam>
		/// <param name="this">The serializer to create the requested instance.</param>
		/// <param name="reader">The reader containing the necessary data to deserialize the object of requested type.</param>
		/// <returns>An instance of the requested type.</returns>
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, TextReader reader)
			=> @this.Deserialize<T>(ExtensionModel.Xml.Defaults.ReaderSettings, reader);

		/// <summary>
		/// Deserialization convenience method to deserialize a document found within the provided <see cref="TextReader"/>
		/// into an instance of the requested instance type, using the provided <see cref="XmlReaderSettings"/>.
		/// </summary>
		/// <typeparam name="T">The requested instance type.</typeparam>
		/// <param name="this">The serializer to create the requested instance.</param>
		/// <param name="settings">The reader settings for handling the xml reader used create the instance.</param>
		/// <param name="reader">The reader containing the necessary data to deserialize the object of requested type.</param>
		/// <returns>An instance of the requested type.</returns>
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, TextReader reader)
			=> @this.Deserialize(new XmlReaderFactory(settings, settings.NameTable.Context()).Get(reader)).AsValid<T>();

		#region Obsolete

		/// <exclude />
		[Obsolete("This is considered deprecated, unsupported functionality and will be removed in a future release.")]
		public static IExtendedXmlSerializer Create<T>(this T @this, Func<T, IConfigurationContainer> configure)
			where T : IConfigurationContainer => configure(@this).Create();

		#endregion
	}
}