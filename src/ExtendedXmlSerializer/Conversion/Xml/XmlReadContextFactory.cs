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

using System.Linq;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlReadContextFactory : IXmlReadContextFactory
	{
		readonly static char[] Delimiters = Defaults.NamespaceDelimiter.ToArray();

		readonly IElements _elements;
		readonly ITypes _types;
		readonly INameConverter _converter;
		readonly XName _type;

		public XmlReadContextFactory(IElements elements, ITypes types, INameConverter converter) : this(elements, types, converter, converter.Get(TypeProperty.Default)) {}

		public XmlReadContextFactory(IElements elements, ITypes types, INameConverter converter, XName type)
		{
			_elements = elements;
			_types = types;
			_converter = converter;
			_type = type;
		}

		public IReadContext Create(IContainerElement container, XElement data)
		{
			var element = Element(container, data);
			var member = container as IMemberElement;
			if (member != null)
			{
				return new XmlReadMemberContext(this, member, element, data);
			}

			var result = new XmlReadContext(this, container, element, data);
			return result;
		}

		IElement Element(IContainerElement container, XElement element)
		{
			var data = Attribute(element, _type);
			if (data != null)
			{
				var name = GetName(data, element);
				var typeInfo = _types.Get(name) ?? container.Classification;
				var result = _elements.Load(container, typeInfo);
				return result;
			}
			return container.Element;
		}

		static XName GetName(string data, XElement element)
		{
			var parts = data.ToStringArray(Delimiters);
			switch (parts.Length)
			{
				case 2:
					var ns = element.GetNamespaceOfPrefix(parts[0])?.NamespaceName ?? string.Empty;
					var result = XName.Get(parts[1], ns);
					return result;
			}
			throw new SerializationException($"Could not parse XML name from {data}");
		}

		public string Value(IName name, XElement data)
		{
			var xName = _converter.Get(name);
			var value = Attribute(data, xName);
			return value;
		}

		static string Attribute(XElement data, XName xName) => data.HasAttributes ? data.Attribute(xName)?.Value : null;
	}
}