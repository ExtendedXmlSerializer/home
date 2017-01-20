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

using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	public class ExtendedXmlSerializer : IExtendedXmlSerializer
	{
		public static ExtendedXmlSerializer Default { get; } = new ExtendedXmlSerializer();
		ExtendedXmlSerializer() : this(new ExtendedXmlConfiguration()) {}

		// ReSharper disable once NotAccessedField.Local
		readonly IExtendedXmlConfiguration _configuration;
		readonly IConverter _converter;
		readonly IElements _elements;

		public ExtendedXmlSerializer(IExtendedXmlConfiguration configuration) : this(configuration, RootConverter.Default) {}

		public ExtendedXmlSerializer(IExtendedXmlConfiguration configuration, IConverter converter) : this(configuration, converter, Elements.Default) {}

		public ExtendedXmlSerializer(IExtendedXmlConfiguration configuration, IConverter converter, IElements elements)
		{
			_configuration = configuration;
			_converter = converter;
			_elements = elements;
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var writer = XmlWriter.Create(stream))
			{
				var root = new Root(_elements.Build(instance.GetType().GetTypeInfo()));
				var context = new XmlWriteContext(writer, root);
				_converter.Write(context, instance);
			}
		}

		public object Deserialize(Stream stream)
		{
			var text = new StreamReader(stream).ReadToEnd();
			var document = XDocument.Parse(text);
			var context = new XmlReadContext(document.Root);
			var result = _converter.Read(context);
			return result;
		}
	}
}