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

using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class OptimizedNamespaceXmlWriter : IXmlWriter
	{
		readonly IXmlWriter _writer;
		readonly ImmutableArray<string> _identifiers;
		readonly ConditionMonitor _condition;

		public OptimizedNamespaceXmlWriter(IXmlWriter writer, ImmutableArray<string> identifiers)
			: this(writer, identifiers, new ConditionMonitor()) {}

		public OptimizedNamespaceXmlWriter(IXmlWriter writer, ImmutableArray<string> identifiers, ConditionMonitor condition)
		{
			_writer = writer;
			_identifiers = identifiers;
			_condition = condition;
		}

		public XmlWriter Get() => _writer.Get();

		public object Root => _writer.Root;

		public void Start(IIdentity identity)
		{
			_writer.Start(identity);

			if (_condition.Apply())
			{
				foreach (var identifier in _identifiers)
				{
					Get(identifier);
				}
			}
		}

		public void EndCurrent() => _writer.EndCurrent();

		public void Content(IIdentity property, string content) => _writer.Content(property, content);
		public void Content(string content) => _writer.Content(content);

		public string Get(MemberInfo parameter) => _writer.Get(parameter);
		public void Dispose() => _writer.Dispose();

		public string Get(string parameter) => _writer.Get(parameter);
	}
}