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
using ExtendedXmlSerialization.ConverterModel.Properties;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class TypeExtractor : ITypeExtractor
	{
		public static TypeExtractor Default { get; } = new TypeExtractor();
		TypeExtractor() : this(Types.Default, TypeProperty.Default, ItemTypeProperty.Default, TypeArgumentsProperty.Default) {}

		readonly ITypes _types;
		readonly ITypeProperty _type;
		readonly ITypeProperty _item;
		readonly ITypeArgumentsProperty _generic;

		public TypeExtractor(ITypes types, ITypeProperty type, ITypeProperty item, ITypeArgumentsProperty generic)
		{
			_types = types;
			_type = type;
			_item = item;
			_generic = generic;
		}

		public TypeInfo Get(IXmlReader parameter)
		{
			if (parameter.Contains(_type.Name))
			{
				return _type.Get(parameter);
			}

			var type = parameter.Contains(_item.Name)
				? _item.Get(parameter).MakeArrayType().GetTypeInfo()
				: Get(parameter.Name);

			var result = type.IsGenericTypeDefinition
				? type.MakeGenericType(_generic.Get(parameter).ToArray()).GetTypeInfo()
				: type;
			return result;
		}

		public TypeInfo Get(XName parameter) => _types.Get(parameter);
	}
}