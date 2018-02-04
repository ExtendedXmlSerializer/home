// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public sealed class XmlSerializationExtension : ISerializerExtension
	{
		readonly XmlReaderSettings _reader;
		readonly XmlWriterSettings _writer;
		readonly XmlNameTable _names;

		public XmlSerializationExtension()
			: this(Defaults.ReaderSettings, Defaults.WriterSettings, new NameTable()) {}

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
			            .RegisterInstance<IIdentifierFormatter>(IdentifierFormatter.Default)
			            .RegisterInstance<IReaderFormatter>(ReaderFormatter.Default)
			            .RegisterInstance<IFormattedContentSpecification>(FormattedContentSpecification.Default)
			            .RegisterInstance<IListContentsSpecification>(
			                                                          new ListContentsSpecification(
			                                                                                        IsTypeSpecification<
					                                                                                        IListInnerContent>
				                                                                                        .Default
				                                                                                        .And(ElementSpecification
					                                                                                             .Default)))
			            .Register<IInnerContentActivation, XmlInnerContentActivation>()
			            .Register<IFormatReaderContexts<XmlNameTable>, FormatReaderContexts>()
			            .Register<IFormatWriters<System.Xml.XmlWriter>, FormatWriters>()
			            .Register<IXmlReaderFactory, XmlReaderFactory>()
			            .Register<IFormatReaders<System.Xml.XmlReader>, FormatReaders>()
			            .Register<IExtendedXmlSerializer, ExtendedXmlSerializer>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}