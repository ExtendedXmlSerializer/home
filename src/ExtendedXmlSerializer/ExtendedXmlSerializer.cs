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
using ExtendedXmlSerialization.Conversion.Elements;
using ExtendedXmlSerialization.Conversion.Xml;
using XmlReader = ExtendedXmlSerialization.Conversion.Xml.XmlReader;


namespace ExtendedXmlSerialization
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	public class ExtendedXmlSerializer : IExtendedXmlSerializer
	{
		readonly static XmlReaderSettings XmlReaderSettings = new XmlReaderSettings
		                                                      {
			                                                      IgnoreWhitespace = true,
			                                                      IgnoreComments = true,
			                                                      IgnoreProcessingInstructions = true
		                                                      };

		readonly IRoots _roots;
		readonly XmlReaderSettings _settings;

		public ExtendedXmlSerializer() : this(Roots.Default) {}

		public ExtendedXmlSerializer(IRoots roots) : this(roots, XmlReaderSettings) {}

		public ExtendedXmlSerializer(IRoots roots, XmlReaderSettings settings)
		{
			_roots = roots;
			_settings = settings;
		}

		public void Serialize(Stream stream, object instance)
		{
			using (var source = XmlWriter.Create(stream))
			{
				var context = _roots.Get(instance.GetType().GetTypeInfo());
				context.Emit(source, instance);
			}
		}

		public object Deserialize(Stream stream)
		{
			using (var source = System.Xml.XmlReader.Create(stream, _settings))
			{
				var reader = new XmlReader(source);
				var context = _roots.Get(reader.Classification());
				var result = context.Get(reader);
				return result;
			}
		}
	}
}