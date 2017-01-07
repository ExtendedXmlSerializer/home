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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Model
{
    public interface ISerializer
    {
        void Serialize(Stream stream, object instance);
    }

    public class Serializer : ISerializer
    {
        private readonly IWriter _writer;

        public Serializer() : this(RootWriter.Default) {}

        public Serializer(IWriter writer)
        {
            _writer = writer;
        }

        public void Serialize(Stream stream, object instance)
        {
            using (var writer = XmlWriter.Create(stream))
            {
                _writer.Write(writer, instance);
            }
        }
    }

    public class RootWriter : ElementWriter
    {
        public static RootWriter Default { get; } = new RootWriter();
        RootWriter() : this(AllNames.Default, SelectingWriter.Default) {}

        public RootWriter(INames names, IWriter body) : base(names.Get, body) {}
    }

    public class InstanceValidatingWriter : DecoratedWriter
    {
        public InstanceValidatingWriter(IWriter writer) : base(writer) {}

        public override void Write(XmlWriter writer, object instance)
        {
            if (instance != null)
            {
                base.Write(writer, instance);
            }
        }
    }

    class SelectingWriter : IWriter
    {
        public static SelectingWriter Default { get; } = new SelectingWriter();
        SelectingWriter() : this(Selectors.Default.Get(Types.Default).Self) {}

        private readonly Func<ISelector> _selector;

        public SelectingWriter(Func<ISelector> selector)
        {
            _selector = selector;
        }

        public void Write(XmlWriter writer, object instance) =>
            _selector().Get(instance.GetType().GetTypeInfo()).Write(writer, instance);
    }

    public interface INameProvider : IParameterizedSource<MemberInfo, XName> {}

    class EnumerableNameProvider : ElementNameProviderBase
    {
        public static EnumerableNameProvider Default { get; } = new EnumerableNameProvider();
        EnumerableNameProvider() : this(ElementTypeLocator.Default, string.Empty) {}

        private readonly IElementTypeLocator _locator;

        public EnumerableNameProvider(IElementTypeLocator locator, string defaultNamespace) : base(defaultNamespace)
        {
            _locator = locator;
        }

        protected override string DetermineName(Typed type)
        {
            var arguments = type.Info.GetGenericArguments();
            var name = arguments.Any()
                ? string.Join(string.Empty, arguments.Select(p => p.Name))
                : _locator.Locate(type).Name;
            var result = $"ArrayOf{name}";
            return result;
        }
    }

    class NameProvider : ElementNameProviderBase
    {
        public static NameProvider Default { get; } = new NameProvider();
        NameProvider() : this(string.Empty) {}

        public NameProvider(string defaultNamespace) : base(defaultNamespace) {}

        protected override string DetermineName(Typed type)
        {
            if (type.Info.IsGenericType)
            {
                var types = type.Info.GetGenericArguments();
                var names = string.Join(string.Empty, types.Select(p => p.Name));
                var result = type.Info.Name.Replace($"`{types.Length.ToString()}", $"Of{names}");
                return result;
            }
            return type.Info.Name;
        }
    }
    abstract class ElementNameProviderBase : NameProviderBase
    {
        private readonly string _defaultNamespace;

        protected ElementNameProviderBase(string defaultNamespace)
        {
            _defaultNamespace = defaultNamespace;
        }

        protected override XName Create(TypeInfo type, MemberInfo member)
        {
            var attribute = type.GetCustomAttribute<XmlRootAttribute>(false);
            var name = attribute?.ElementName.NullIfEmpty() ?? DetermineName(new Typed(type));
            var ns = attribute?.Namespace ?? _defaultNamespace;
            var result = XName.Get(name, ns);
            return result;
        }

        protected abstract string DetermineName(Typed type);
    }

    public class EnumerableBodyWriter : WriterBase<IEnumerable>
    {
        private readonly IWriter _writer;

        public EnumerableBodyWriter(IWriter itemWriter)
            : this(AllNames.Default, itemWriter) {}

        public EnumerableBodyWriter(INames names, IWriter itemWriter)
            : this(new ElementWriter(names.Get, itemWriter)) {}

        EnumerableBodyWriter(ElementWriter writer)
        {
            _writer = writer;
        }

        protected override void Write(XmlWriter writer, IEnumerable instance)
        {
            foreach (var item in instance)
            {
                _writer.Write(writer, item);
            }
        }
    }

    abstract class NameProviderBase : WeakCacheBase<MemberInfo, XName>, INameProvider
    {
        protected override XName Create(MemberInfo parameter)
        {
            var typeInfo = parameter as TypeInfo ?? parameter.DeclaringType.GetTypeInfo();
            var result = Create(typeInfo, parameter);
            return result;
        }

        protected abstract XName Create(TypeInfo type, MemberInfo member);
    }

    class MemberNameProvider : NameProviderBase
    {
        public static MemberNameProvider Default { get; } = new MemberNameProvider();
        MemberNameProvider() {}

        protected override XName Create(TypeInfo type, MemberInfo member)
        {
            var result = member.GetCustomAttribute<XmlAttributeAttribute>(false)?.AttributeName.NullIfEmpty() ??
                         member.GetCustomAttribute<XmlElementAttribute>(false)?.ElementName.NullIfEmpty() ??
                         member.Name;
            return result;
        }
    }

    public abstract class WriterBase<T> : IWriter
    {
        protected abstract void Write(XmlWriter writer, T instance);

        void IWriter.Write(XmlWriter writer, object instance)
            => Write(writer, (T) instance);
    }

    public abstract class WriterBase : IWriter
    {
        public abstract void Write(XmlWriter context, object instance);
    }


    /*public interface IWriterContext
    {
        XmlWriter Writer { get; }

        void StartNewContext(object instance);
    }

    class WriterContext : IWriterContext
    {
        private readonly IWriter _writer;

        public WriterContext(IWriter writer, XmlWriter xmlWriter)
        {
            _writer = writer;
            Writer = xmlWriter;
        }

        public void StartNewContext(object instance) => _writer.Write(Writer, instance);
        public XmlWriter Writer { get; }
    }

    public interface IEnhancedWriter
    {
        void Write(IWriterContext context, object instance, Typed type);
    }*/

    /*public class ValueWriter : ValueWriter<object>
    {
        public ValueWriter(Func<object, string> serialize) : base(serialize) {}
    }*/

    public class ValueWriter<T> : WriterBase<T>
    {
        private readonly Func<T, string> _serialize;

        public ValueWriter(Func<T, string> serialize)
        {
            _serialize = serialize;
        }

        protected override void Write(XmlWriter writer, T instance) => writer.WriteString(_serialize(instance));
    }

    public class DecoratedWriter : WriterBase
    {
        private readonly IWriter _writer;

        public DecoratedWriter(IWriter writer)
        {
            _writer = writer;
        }

        public override void Write(XmlWriter writer, object instance)
            => _writer.Write(writer, instance);
    }

    public abstract class ElementWriterBase : DecoratedWriter
    {
        protected ElementWriterBase(IWriter writer) : base(writer) {}

        public override void Write(XmlWriter writer, object instance)
        {
            var name = Get(instance.GetType().GetTypeInfo());
            writer.WriteStartElement(name.LocalName, name.NamespaceName);
            base.Write(writer, instance);
            writer.WriteEndElement();
        }

        protected abstract XName Get(TypeInfo type);
    }

    public class ElementWriter : ElementWriterBase
    {
        private readonly Func<TypeInfo, XName> _name;

        public ElementWriter(Func<TypeInfo, XName> name, IWriter writer) : base(writer)
        {
            _name = name;
        }

        protected override XName Get(TypeInfo info) => _name(info);
    }
}