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

using ExtendedXmlSerializer.ContentModel.Properties;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	class XmlFactory : IXmlFactory
	{
		readonly IGenericTypes _genericTypes;
		readonly ITypes _types;
		readonly ITypeProperty _type;
		readonly IItemTypeProperty _item;
		readonly IArgumentsProperty _arguments;

		public XmlFactory(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
		                  IArgumentsProperty arguments)
		{
			_genericTypes = genericTypes;
			_types = types;
			_type = type;
			_item = item;
			_arguments = arguments;
		}

		public IXmlWriter Create(System.Xml.XmlWriter writer, object instance) => new XmlWriter(writer, instance);

		public IXmlReader Create(System.Xml.XmlReader reader) =>
			new XmlReader(_genericTypes, _types, _type, _item, _arguments, reader);
	}

	/*public interface IInstanceParser : IParser<object> {}

	class InstanceParser : IInstanceParser
	{
		readonly static Encoding Encoding = Encoding.UTF8;
		readonly IExtendedXmlSerializer _serializer;
		readonly Encoding _encoding;

		public InstanceParser(IExtendedXmlSerializer serializer) : this(serializer, Encoding) {}

		public InstanceParser(IExtendedXmlSerializer serializer, Encoding encoding)
		{
			_serializer = serializer;
			_encoding = encoding;
		}

		public object Get(string parameter)
		{
			var bytes = _encoding.GetBytes(parameter);
			using (var stream = new MemoryStream(bytes))
			{
				var result = _serializer.Deserialize(stream);
				return result;
			}
		}
	}*/
}