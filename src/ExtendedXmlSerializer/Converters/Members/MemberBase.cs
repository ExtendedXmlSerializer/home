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
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Converters.Read;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Converters.Members
{
    public abstract class MemberBase : IMember
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;
        private readonly Typed _memberType;
        private readonly Func<object, object> _getter;

        protected MemberBase(IReader reader, IWriter writer, XName name, Typed memberType, Func<object, object> getter)
        {
            Name = name;
            _reader = reader;
            _writer = writer;
            _memberType = memberType;
            _getter = getter;
        }

        public XName Name { get; }

        public void Write(XmlWriter writer, object instance) => _writer.Write(writer, Get(instance));

        protected virtual object Get(object instance) => _getter(instance);

        public object Read(XElement element, Typed? hint = null) => _reader.Read(element, hint ?? _memberType);
    }
}