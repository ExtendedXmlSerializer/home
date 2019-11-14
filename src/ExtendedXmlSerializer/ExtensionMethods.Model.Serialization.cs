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
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		readonly static Func<Stream> New = DefaultActivators.Default.New<MemoryStream>;

		readonly static IXmlWriterFactory WriterFactory
			= new XmlWriterFactory(CloseSettings.Default.Get(ExtensionModel.Xml.Defaults.WriterSettings));

		public static string Serialize(this IExtendedXmlSerializer @this, object instance)
			=> @this.Serialize(WriterFactory, New, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, object instance)
			=> @this.Serialize(new XmlWriterFactory(CloseSettings.Default.Get(settings)), New, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, Stream stream, object instance)
			=> @this.Serialize(XmlWriterFactory.Default, stream.Self, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, Stream stream,
		                               object instance)
			=> @this.Serialize(new XmlWriterFactory(settings), stream.Self, instance);

		static string Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, Func<Stream> stream,
		                        object instance)
			=> new InstanceFormatter(@this, factory, stream).Get(instance);

		public static void Serialize(this IExtendedXmlSerializer @this, TextWriter writer, object instance)
			=> @this.Serialize(XmlWriterFactory.Default, writer, instance);

		public static void Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, TextWriter writer,
		                             object instance)
			=> @this.Serialize(new XmlWriterFactory(settings), writer, instance);

		static void Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, TextWriter writer,
		                      object instance)
			=> @this.Serialize(factory.Get(writer), instance);

		public static XmlParserContext Context(this XmlNameTable @this)
			=> XmlParserContexts.Default.Get(@this ?? new NameTable());

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, string data)
			=> @this.Deserialize<T>(ExtensionModel.Xml.Defaults.CloseRead, data);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, string data)
			=> @this.Deserialize<T>(settings, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, Stream stream)
			=> @this.Deserialize<T>(ExtensionModel.Xml.Defaults.ReaderSettings, stream);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, Stream stream)
			=> @this.Deserialize<T>(new XmlReaderFactory(settings, settings.NameTable.Context()), stream);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, Stream stream)
			=> @this.Deserialize(factory.Get(stream)).AsValid<T>();

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, TextReader reader)
			=> @this.Deserialize<T>(ExtensionModel.Xml.Defaults.ReaderSettings, reader);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, TextReader reader)
			=> @this.Deserialize<T>(new XmlReaderFactory(settings, settings.NameTable.Context()), reader);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, TextReader reader)
			=> @this.Deserialize(factory.Get(reader)).AsValid<T>();

		#region Obsolete

		[Obsolete("This is considered deprecated, unsupported functionality and will be removed in a future release.")]
		public static IExtendedXmlSerializer Create<T>(this T @this, Func<T, IConfigurationContainer> configure)
			where T : IConfigurationContainer => configure(@this).Create();

		#endregion
	}
}
