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

using System.Text;
using System.Xml;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class XmlSerializationExtension : ISerializerExtension
	{
		readonly XmlReaderSettings _reader;
		readonly XmlWriterSettings _writer;
		readonly XmlNameTable _names;

		public XmlSerializationExtension()
			: this(ContentModel.Xml.Defaults.ReaderSettings, ContentModel.Xml.Defaults.WriterSettings, new NameTable()) {}

		public XmlSerializationExtension(XmlReaderSettings reader, XmlWriterSettings writer, XmlNameTable names)
		{
			_reader = reader;
			_writer = writer;
			_names = names;
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(Encoding.UTF8)
			            .RegisterInstance(_names)
			            .RegisterInstance(_reader.Clone())
			            .RegisterInstance(_writer.Clone())
			            .RegisterInstance<IContentAdapterFormatter>(ContentAdapterFormatter.Default)
			            .RegisterInstance<IPropertyContentSpecification>(PropertyContentSpecification.Default)
			            .RegisterInstance<IListContentsSpecification>(
				            new ListContentsSpecification(
					            IsTypeSpecification<IListContentsAdapter>.Default.And(ElementSpecification.Default)))
			            .Register<IContentsActivation, XmlContentsActivation>()
			            .Register<IXmlReaderContexts, XmlReaderContexts>()
			            .Register<IXmlReaderFactory, XmlReaderFactory>()
			            .Register<IXmlFactory, XmlFactory>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}