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
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Model
{
    public interface IDeserializer
    {
        object Deserialize(Stream stream);
    }

    public class Deserializer<T> : IDeserializer
    {
        private readonly IDeserializer _deserializer;

        public Deserializer() : this(new Deserializer(typeof(T))) {}

        public Deserializer(IDeserializer deserializer)
        {
            _deserializer = deserializer;
        }

        public T Deserialize(Stream stream) => (T) _deserializer.Deserialize(stream);

        object IDeserializer.Deserialize(Stream stream) => Deserialize(stream);
    }

    public class Deserializer : IDeserializer
    {
        private readonly IReader _reader;

        public Deserializer() : this(RootReader.Default) {}

        public Deserializer(Type type)
            : this(new HintedRootTypes(type)) {}

        public Deserializer(ITypes types)
            : this(new RootReader(new SelectingReader(types, new Selector(types)))) {}

        public Deserializer(IReader reader)
        {
            _reader = reader;
        }

        public object Deserialize(Stream stream)
        {
            var text = new StreamReader(stream).ReadToEnd();
            var element = XDocument.Parse(text).Root;
            var result = _reader.Read(element);
            return result;
        }
    }

    public interface IReader
    {
        object Read(XElement element);
    }

    class RootReader : DecoratedReader
    {
        public static RootReader Default { get; } = new RootReader();
        RootReader() : this(SelectingReader.Default) {}

        public RootReader(IReader reader) : base(reader) {}
    }

    class SelectingReader : IReader
    {
        public static SelectingReader Default { get; } = new SelectingReader();
        SelectingReader() : this(Types.Default, Selector.Default) {}

        private readonly ITypes _types;
        private readonly ISelector _selector;

        public SelectingReader(ITypes types, ISelector selector)
        {
            _types = types;
            _selector = selector;
        }

        public object Read(XElement element)
        {
            var info = _types.Get(element).GetTypeInfo();
            var converter = _selector.Get(info);
            var result = converter.Read(element);
            return result;
        }
    }

    public abstract class ReaderBase : IReader
    {
        public abstract object Read(XElement element);
    }

    public abstract class ReaderBase<T> : IReader
    {
        object IReader.Read(XElement element) => Read(element);

        protected abstract T Read(XElement element);
    }

    public class ValueReader<T> : ReaderBase<T>
    {
        private readonly Func<string, T> _deserialize;

        public ValueReader(Func<string, T> deserialize)
        {
            _deserialize = deserialize;
        }

        protected override T Read(XElement element) => _deserialize(element.Value);
    }

    public class DecoratedReader : ReaderBase
    {
        private readonly IReader _reader;

        public DecoratedReader(IReader reader)
        {
            _reader = reader;
        }

        public override object Read(XElement element) => _reader.Read(element);
    }
}