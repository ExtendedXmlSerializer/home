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
using System.Reflection;
using ExtendedXmlSerialization.Conversion.ElementModel;

namespace ExtendedXmlSerialization.Conversion.Read
{
    public class XmlReadContainerContext<T> : IReadContainerContext<T> where T : IDeclaredTypeElement
    {
        private readonly IReadContext _context;

        public XmlReadContainerContext(T container, IReadContext context)
        {
            _context = context;
            Container = container;
        }

        public T Container { get; }
        IDeclaredTypeElement IReadContainerContext.Container => Container;

        public IEnumerator<IReadMemberContext> GetEnumerator() => _context.GetEnumerator();

        public string DisplayName => _context.DisplayName;

        public TypeInfo Classification => _context.Classification;

        public object GetService(Type serviceType) => _context.GetService(serviceType);

        public void Add(object service) => _context.Add(service);

        public IElement Element => _context.Element;

        public IReadContext Member(IElement element) => _context.Member(element);

        public IEnumerable<IReadContext> Items() => _context.Items();

        public string Read() => _context.Read();

        public string this[IElementName name] => _context[name];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}