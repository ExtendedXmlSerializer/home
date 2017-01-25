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
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class OptimizedWriteContext : IWriteContext
	{
		const string Prefix = "xmlns";

		readonly IObjectNamespaces _namespaces;
		readonly IWriteContext _context;
		readonly XmlWriter _writer;
		readonly object _instance;

		public OptimizedWriteContext(IObjectNamespaces namespaces, IWriteContext context, XmlWriter writer, object instance)
		{
			_namespaces = namespaces;
			_context = context;
			_writer = writer;
			_instance = instance;
		}

		public object GetService(Type serviceType) => _context.GetService(serviceType);

		public IContainerElement Container => _context.Container;
		public IElement Element => _context.Element;

		public IWriteContext New(IContainerElement container, TypeInfo instanceType) => _context.New(container, instanceType);

		public IDisposable Emit()
		{
			_writer.WriteStartDocument();
			var result = _context.Emit();
			var names = _namespaces.Get(_instance);

			for (var i = 0; i < names.Length; i++)
			{
				var name = names[i];
				switch (i)
				{
					case 0:
						_writer.WriteAttributeString(Prefix, name.NamespaceName);
						break;
					default:
						_writer.WriteAttributeString(Prefix, name.LocalName, string.Empty, name.NamespaceName);
						break;
				}
			}

			return result;
		}

		public void Write(string text) => _context.Write(text);

		public void Write(IName name, object value) => _context.Write(name, value);
	}
}