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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class Namespaces : CacheBase<TypeInfo, INamespace>, INamespaces
	{
		public Namespaces() : this(PrefixProvider.Default) {}

		readonly IDictionary<Assembly, INamespace> _known;
		readonly ITypeFormatter _formatter;
		readonly IPrefixProvider _prefix;

		public Namespaces(IPrefixProvider prefix) : this(KnownNamespaces.Default, NamespaceFormatter.Default, prefix) {}

		public Namespaces(IDictionary<Assembly, INamespace> known, ITypeFormatter formatter, IPrefixProvider prefix)
		{
			_known = known;
			_formatter = formatter;
			_prefix = prefix;
		}

		protected override INamespace Create(TypeInfo parameter) => _known.TryGet(parameter.Assembly) ?? Format(parameter);

		Namespace Format(TypeInfo parameter)
		{
			var name = _formatter.Get(parameter);
			var result = new Namespace(_prefix.Get(name), name);
			return result;
		}
	}
}