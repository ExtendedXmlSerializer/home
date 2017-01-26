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

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class ObjectNamespaces : IObjectNamespaces
	{
		public ObjectNamespaces(IElements elements) : this(elements, new Namespaces(new GeneratedPrefixProvider()).Get) {}

		readonly IElements _elements;
		readonly Func<TypeInfo, XName> _namespaces;

		public ObjectNamespaces(IElements elements, Func<TypeInfo, XName> namespaces)
		{
			_elements = elements;
			_namespaces = namespaces;
		}

		public ImmutableArray<XName> Get(object parameter)
			=> new ObjectTypeWalker(_elements, parameter)
				.Get()
				.Distinct()
				.Select(_namespaces)
				.Distinct()
				.ToImmutableArray();
	}
}