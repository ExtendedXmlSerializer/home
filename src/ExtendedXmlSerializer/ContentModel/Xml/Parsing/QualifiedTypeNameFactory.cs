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
using System.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Xml.Parsing
{
	class QualifiedTypeNameFactory : IParameterizedSource<TypeInfo, QualifiedNameParts>
	{
		readonly INames _names;
		readonly IXmlNamespaceResolver _resolver;
		readonly Func<TypeInfo, QualifiedNameParts> _selector;

		public QualifiedTypeNameFactory(IXmlNamespaceResolver resolver) : this(Names.Default, resolver) {}

		QualifiedTypeNameFactory(INames names, IXmlNamespaceResolver resolver)
		{
			_names = names;
			_resolver = resolver;
			_selector = Get;
		}

		public QualifiedNameParts Get(TypeInfo parameter)
		{
			var name = _names.Get(parameter);
			var @namespace = _resolver.LookupPrefix(name.NamespaceName);
			var result = parameter.IsGenericType
				? new QualifiedNameParts(@namespace, name.LocalName,
				                         parameter.GetGenericArguments().YieldMetadata().Select(_selector).ToImmutableArray)
				: new QualifiedNameParts(@namespace, name.LocalName);

			return result;
		}
	}
}