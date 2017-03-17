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
using System.Xml;
using ExtendedXmlSerialization.ContentModel.Properties;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	class XmlFactory : IXmlFactory
	{
		readonly IGenericTypes _genericTypes;
		readonly ITypes _types;
		readonly ITypeProperty _type;
		readonly IItemTypeProperty _item;
		readonly IArgumentsProperty _arguments;
		readonly XmlReaderSettings _readerSettings;
		readonly XmlWriterSettings _writerSettings;

		public XmlFactory(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
		                  IArgumentsProperty arguments, XmlReaderSettings readerSettings, XmlWriterSettings writerSettings)
		{
			_genericTypes = genericTypes;
			_types = types;
			_type = type;
			_item = item;
			_arguments = arguments;
			_readerSettings = readerSettings;
			_writerSettings = writerSettings;
		}

		public IXmlWriter Create(Stream stream, object instance)
			=> new XmlWriter(System.Xml.XmlWriter.Create(stream, _writerSettings), instance);

		public IXmlReader Create(Stream stream) =>
			new XmlReader(_genericTypes, _types, _type, _item, _arguments, System.Xml.XmlReader.Create(stream, _readerSettings));
	}
}