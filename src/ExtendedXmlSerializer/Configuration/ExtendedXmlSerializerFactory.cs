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

using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Configuration
{
	class ExtendedXmlSerializerFactory : IExtendedXmlSerializerFactory
	{
		public static ExtendedXmlSerializerFactory Default { get; } = new ExtendedXmlSerializerFactory();
		ExtendedXmlSerializerFactory() {}

		public IExtendedXmlSerializer Get(IExtendedXmlConfiguration parameter)
		{
			var namespaces = new Namespaces();
			var locator = new CollectionItemTypeLocator();
			var add = new AddDelegates(locator, new AddMethodLocator());
			var elements = new Elements(locator, add);
			var types = new Types(namespaces, new TypeContexts());
			var factory = new XmlContextFactory(elements, namespaces, types);
			var converter = new RootConverter(new ConverterOptions(add));
			var result = new ExtendedXmlSerializer(factory, converter);
			return result;
		}
	}
}