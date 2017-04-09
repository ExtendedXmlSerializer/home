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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Xml;
using JetBrains.Annotations;
using XmlReader = System.Xml.XmlReader;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	[UsedImplicitly]
	sealed class ExtendedXmlSerializer : IExtendedXmlSerializer
	{
		readonly IFormatReaderFactory _readers;
		readonly IFormatWriterFactory _writers;
		readonly IClassification _classification;
		readonly ISerializers _serializers;

		public ExtendedXmlSerializer(IFormatReaderFactory readers, IFormatWriterFactory writers,
		                             IClassification classification, ISerializers serializers)
		{
			_readers = readers;
			_writers = writers;
			_classification = classification;
			_serializers = serializers;
		}

		public void Serialize(XmlWriter writer, object instance)
			=> _serializers.Get(instance.GetType().GetTypeInfo())
			               .Write(_writers.Create(writer, instance), instance);

		public object Deserialize(XmlReader reader)
		{
			using (var content = _readers.Get(reader))
			{
				var classification = _classification.GetClassification(content);
				var result = _serializers.Get(classification).Get(content);
				return result;
			}
		}
	}
}