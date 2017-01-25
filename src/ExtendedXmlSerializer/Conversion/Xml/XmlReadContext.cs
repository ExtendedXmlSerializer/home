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
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Xml
{
	public class XmlReadContext : XmlReadContext<IContainerElement>
	{
		public XmlReadContext(IXmlReadContextFactory factory, IContainerElement container, XElement data) : base(factory, container, data) {}
		public XmlReadContext(IXmlReadContextFactory factory, IContainerElement container, IElement element, XElement data) : base(factory, container, element, data) {}
	}

	public abstract class XmlReadContext<T> : IReadContext<T>, IXmlReadContext where T : class, IContainerElement
	{
		readonly IXmlReadContextFactory _factory;

		protected XmlReadContext(IXmlReadContextFactory factory, T container, XElement data) : this(factory, container, container.Element, data) {}

		protected XmlReadContext(IXmlReadContextFactory factory, T container, IElement element, XElement data)
		{
			_factory = factory;
			Container = container;
			Element = element;
			Data = data;
		}

		public XElement Data { get; }

		public T Container { get; }
		public IElement Element { get; }
		IContainerElement IContext.Container => Container;

		public object GetService(Type serviceType) => Data.AnnotationAll(serviceType);

		public void Add(object service) => Data.AddAnnotation(service);

		public string Read() => Data.Value;

		public IEnumerable<IReadContext> Items()
		{
			var element = (ICollectionElement) Element;
			var container = element.Element;
			foreach (var child in Data.Elements())
			{
				yield return _factory.Create(container, child);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IReadMemberContext> GetEnumerator()
		{
			var members = ((IMemberedElement) Element).Members;
			foreach (var source in Data.Elements())
			{
				var member = members.Get(source.Name.LocalName);
				if (member != null)
				{
					yield return (IReadMemberContext) _factory.Create(member, source);
				}
			}
		}

		public string this[IName name] => _factory.Value(name, Data);
		
	}
}