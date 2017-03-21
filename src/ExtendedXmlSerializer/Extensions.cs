// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;
using System.Text;
using System.Xml;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;
using Defaults = ExtendedXmlSerializer.ContentModel.Xml.Defaults;

namespace ExtendedXmlSerializer
{
	public static class Extensions
	{
		readonly static Func<Stream> New = Activators.Default.New<MemoryStream>;

		readonly static IXmlWriterFactory WriterFactory
			= new XmlWriterFactory(CloseSettings.Default.Get(Defaults.WriterSettings));

		readonly static IXmlReaderFactory ReaderFactory
			= new XmlReaderFactory(CloseSettings.Default.Get(Defaults.ReaderSettings));

		public static IExtendedXmlSerializer Create<T>(this T @this, Func<T, IConfiguration> configure)
			where T : IConfiguration => configure(@this).Create();


		public static string Serialize(this IExtendedXmlSerializer @this, object instance)
			=> Serialize(@this, WriterFactory, New, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, object instance)
			=> Serialize(@this, new XmlWriterFactory(CloseSettings.Default.Get(settings)), New, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, Stream stream, object instance)
			=> Serialize(@this, XmlWriterFactory.Default, stream.Self, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, Stream stream,
		                               object instance)
			=> Serialize(@this, new XmlWriterFactory(settings), stream.Self, instance);

		static string Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, Func<Stream> stream,
		                        object instance)
			=> new InstanceFormatter(@this, factory, stream).Get(instance);

		public static void Serialize(this IExtendedXmlSerializer @this, TextWriter writer, object instance)
			=> Serialize(@this, XmlWriterFactory.Default, writer, instance);

		public static void Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, TextWriter writer,
		                             object instance)
			=> Serialize(@this, new XmlWriterFactory(settings), writer, instance);

		static void Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, TextWriter writer, object instance)
			=> @this.Serialize(factory.Get(writer), instance);


		public static T Deserialize<T>(this IExtendedXmlSerializer @this, string data)
			=> Deserialize<T>(@this, ReaderFactory, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, Stream stream)
			=> Deserialize<T>(@this, XmlReaderFactory.Default, stream);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, Stream stream)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings), stream);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, Stream stream)
			=> @this.Deserialize(factory.Get(stream)).AsValid<T>();

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, TextReader reader)
			=> Deserialize<T>(@this, XmlReaderFactory.Default, reader);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, TextReader reader)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings), reader);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, TextReader reader)
			=> @this.Deserialize(factory.Get(reader)).AsValid<T>();

		class CloseSettings : IAlteration<XmlWriterSettings>, IAlteration<XmlReaderSettings>
		{
			public static CloseSettings Default { get; } = new CloseSettings();
			CloseSettings() {}

			public XmlWriterSettings Get(XmlWriterSettings parameter)
			{
				var result = parameter.Clone();
				result.CloseOutput = true;
				return result;
			}

			public XmlReaderSettings Get(XmlReaderSettings parameter)
			{
				var result = parameter.Clone();
				result.CloseInput = true;
				return result;
			}
		}
	}
}