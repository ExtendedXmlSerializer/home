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

using System.Xml;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	sealed class XmlSerializationExtension : ISerializerExtension
	{
		readonly static XmlReaderSettings ReaderSettings = new XmlReaderSettings
		                                                   {
			                                                   IgnoreWhitespace = true,
			                                                   IgnoreComments = true,
			                                                   IgnoreProcessingInstructions = true
		                                                   };

		readonly static XmlWriterSettings WriterSettings = new XmlWriterSettings();

		public static XmlSerializationExtension Default { get; } = new XmlSerializationExtension();
		XmlSerializationExtension() : this(ReaderSettings, WriterSettings) {}

		readonly XmlReaderSettings _readerSettings;
		readonly XmlWriterSettings _writerSettings;

		public XmlSerializationExtension(XmlWriterSettings writerSettings) : this(ReaderSettings, writerSettings) {}

		public XmlSerializationExtension(XmlReaderSettings readerSettings) : this(readerSettings, WriterSettings) {}

		public XmlSerializationExtension(XmlReaderSettings readerSettings, XmlWriterSettings writerSettings)
		{
			_readerSettings = readerSettings;
			_writerSettings = writerSettings;
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance(_readerSettings).RegisterInstance(_writerSettings);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}