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
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class ElementTypes : IElementTypes
	{
		public static ElementTypes Default { get; } = new ElementTypes();
		ElementTypes() : this(NameConverter.Default) {}

		readonly INameConverter _name;
		readonly XName _type;

		public ElementTypes(INameConverter name) : this(name, name.Get(TypeProperty.Default)) {}

		public ElementTypes(INameConverter name, XName type)
		{
			_type = type;
			_name = name;
		}

		public TypeInfo Get(XElement parameter) => parameter.Annotation<TypeInfo>() ?? Resolve(parameter);

		TypeInfo Resolve(XElement parameter)
		{
			var name = FromAttribute(parameter) ?? parameter.Name;

			var result = _name.Get(name);
			if (result != null)
			{
				parameter.AddAnnotation(result);
				return result;
			}
			return null;
		}

		XName FromAttribute(XElement parameter)
		{
			var name = parameter.Attribute(_type)?.Value;
			if (name != null)
			{
				var parts = name.ToStringArray(Defaults.NamespaceDelimiter.ToArray());
				switch (parts.Length)
				{
					case 2:
						var ns = parameter.Document.Root.GetNamespaceOfPrefix(parts[0])?.NamespaceName;
						var result = XName.Get(parts[1], ns ?? string.Empty);
						return result;
				}
				throw new SerializationException($"Could not parse XML name from {name}");
			}
			return null;
		}
	}
}