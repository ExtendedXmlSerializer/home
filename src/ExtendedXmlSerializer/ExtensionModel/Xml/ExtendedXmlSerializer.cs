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
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Reflection;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// Extended Xml Serializer
	/// </summary>
	[UsedImplicitly]
	sealed class ExtendedXmlSerializer : IExtendedXmlSerializer
	{
		readonly IFormatReaders<System.Xml.XmlReader> _readers;
		readonly IFormatWriters<System.Xml.XmlWriter> _writers;
		readonly IClassification _classification;
		readonly ISerializers _serializers;

		public ExtendedXmlSerializer(IFormatReaders<System.Xml.XmlReader> readers,
		                             IFormatWriters<System.Xml.XmlWriter> writers,
		                             IClassification classification, ISerializers serializers)
		{
			_readers = readers;
			_writers = writers;
			_classification = classification;
			_serializers = serializers;
		}

		public void Serialize(System.Xml.XmlWriter writer, object instance)
			=> _serializers.Get(instance.GetType().GetTypeInfo())
			               .Write(_writers.Get(new Writing<System.Xml.XmlWriter>(writer, instance)), instance);

		public object Deserialize(System.Xml.XmlReader reader)
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