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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ExtensionModel
{
	class Instances : IEnumerable<object>
	{
		readonly IMemberConfiguration _configuration;
		readonly ImmutableArray<object> _additional;

		public Instances(IMemberConfiguration configuration)
			: this(configuration, new XmlReaderSettings(), new XmlWriterSettings()) {}

		public Instances(IMemberConfiguration configuration, params object[] additional)
			: this(configuration,
			       new object[]
			       {
				       configuration.Policy.And<PropertyInfo>(configuration.Specification),
				       configuration.Policy.And<FieldInfo>(configuration.Specification)
			       }.Concat(additional).ToImmutableArray()
			) {}

		public Instances(IMemberConfiguration configuration, ImmutableArray<object> additional)
		{
			_configuration = configuration;
			_additional = additional;
		}

		public IEnumerator<object> GetEnumerator()
		{
			yield return _configuration;
			yield return _configuration.Converters;
			yield return _configuration.Runtime;
			yield return _configuration.Specification;
			yield return _configuration.Aliases;
			yield return _configuration.Order;

			foreach (var o in _additional)
			{
				yield return o;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}