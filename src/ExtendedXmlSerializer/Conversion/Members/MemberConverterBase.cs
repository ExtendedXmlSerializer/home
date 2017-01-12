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
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public abstract class MemberConverterBase : IMemberConverter
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;
        private readonly IMemberElement _element;
        private readonly Func<object, object> _getter;

        protected MemberConverterBase(IReader reader, IWriter writer, IMemberElement element,
                                      Func<object, object> getter)
        {
            _reader = reader;
            _writer = writer;
            _element = element;
            _getter = getter;
        }

        public void Write(IWriteContext context, object instance) => _writer.Write(context, Get(instance));

        protected object Get(object instance) => _getter(instance);

        public object Read(IReadContext context) => _reader.Read(context);

        public string Name => _element.Name;

        public TypeInfo ReferencedType => _element.ReferencedType;

        public bool Assignable => _element.Assignable;

        public MemberInfo Metadata => _element.Metadata;

        public TypeInfo DeclaredType => _element.DeclaredType;
    }
}