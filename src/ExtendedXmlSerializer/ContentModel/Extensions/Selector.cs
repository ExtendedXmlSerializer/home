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

using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ContentModel.Xml.Namespacing;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Extensions
{
	class Selector : IElementOptionSelector
	{
		readonly IObjectNamespaces _namespaces;
		readonly IElementOptionSelector _selector;

		public Selector(IObjectNamespaces namespaces, IElementOptionSelector selector)
		{
			_namespaces = namespaces;
			_selector = selector;
		}

		public IElementOption Get(IContentOption parameter) => new Option(_namespaces, _selector.Get(parameter));

		class Option : IElementOption
		{
			readonly IObjectNamespaces _namespaces;
			readonly IElementOption _option;

			public Option(IObjectNamespaces namespaces, IElementOption option)
			{
				_namespaces = namespaces;
				_option = option;
			}

			public bool IsSatisfiedBy(TypeInfo parameter) => _option.IsSatisfiedBy(parameter);

			public IWriter Get(TypeInfo parameter) => new Writer(_namespaces, _option.Get(parameter));
		}

		class Writer : ReferenceCache<IXmlWriter, ConditionMonitor>, IWriter
		{
			readonly static string Prefix = nameof(XNamespace.Xmlns).ToLower();

			readonly IObjectNamespaces _namespaces;
			readonly IWriter _writer;

			public Writer(IObjectNamespaces namespaces, IWriter writer) : base(_ => new ConditionMonitor())
			{
				_namespaces = namespaces;
				_writer = writer;
			}

			public void Write(IXmlWriter writer, object instance)
			{
				_writer.Write(writer, instance);

				var apply = instance == writer.Root && Get(writer).Apply();
				if (apply)
				{
					var namespaces = _namespaces.Get(writer.Root);
					var length = namespaces.Length;
					for (var i = 0; i < length; i++)
					{
						var attribute = Attribute(namespaces[i], i);
						writer.Attribute(attribute);
					}
				}
			}

			static Attribute Attribute(Namespace ns, int index)
			{
				switch (index)
				{
					case 0:
						return new Attribute(Prefix, ns.Identifier);
					default:
						return new Attribute(ns.Name, ns.Identifier, new Namespace(Prefix));
				}
			}
		}
	}
}