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

    abstract class InstanceWriterBase : ElementWriter
    {
        protected InstanceWriterBase(IWriter writer) : this(NameProvider.Default, writer) {}

        protected InstanceWriterBase(INameProvider provider, IWriter writer) : base(provider.Get, writer) {}
    }

    public class MemberWriter : WriterBase
    {
        public override void Write(XmlWriter writer, object instance) {}
    }

    public interface INameProvider : IParameterizedSource<MemberInfo, XName> {}

    /*class GenericNameProvider : NameProvider
    {
        protected override XName GetName(MemberInfo key, TypeInfo typeInfo)
        {
        var typeInfo = key as TypeInfo ?? key.DeclaringType.GetTypeInfo();
            
            var name = base.GetName(key, typeInfo);
            var isGenericType = typeInfo.IsGenericType;
            if (isGenericType)
            {
                var types = typeInfo.GetGenericArguments();
                var names = string.Join(string.Empty, types.Select(p => p.Name));
                var result = name.LocalName.Replace($"`{types.Length.ToString()}", $"Of{names}");
                return result;
            }


            return name;
        }
    }

    class EnumerableNameProvider : GenericNameProvider
    {
        protected override XName GetName(MemberInfo key, TypeInfo typeInfo)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeInfo.AsType()))
            {
                var result = typeInfo.IsArray || isGenericType
                    ? $"ArrayOf{string.Join(string.Empty, GenericArguments.Select(p => p.Name))}"
                    : name;
                return result;
            }
            return base.GetName(key, typeInfo);
        }
    }*/

    class NameProvider : WeakCacheBase<MemberInfo, XName>, INameProvider
    {
        public static NameProvider Default { get; } = new NameProvider();
        NameProvider() : this(string.Empty) {}

        private readonly string _defaultNamespace;

        public NameProvider(string defaultNamespace)
        {
            _defaultNamespace = defaultNamespace;
        }

        protected override XName Create(MemberInfo parameter)
        {
            var typeInfo = parameter as TypeInfo ?? parameter.DeclaringType.GetTypeInfo();
            var attribute = typeInfo.GetCustomAttribute<XmlRootAttribute>(false);
            var name = attribute?.ElementName.NullIfEmpty() ?? typeInfo.Name;
            var ns = attribute?.Namespace ?? _defaultNamespace;
            var result = XName.Get(name, ns);
            return result;
        }
    }

    class MemberNameProvider : WeakCacheBase<MemberInfo, XName>, INameProvider
    {
        public static MemberNameProvider Default { get; } = new MemberNameProvider();
        MemberNameProvider() {}

        protected override XName Create(MemberInfo parameter)
        {
            var result = parameter.GetCustomAttribute<XmlAttributeAttribute>(false)?.AttributeName.NullIfEmpty() ??
                         parameter.GetCustomAttribute<XmlElementAttribute>(false)?.ElementName.NullIfEmpty() ?? parameter.Name;
            return result;
        }
    }

    public abstract class WriterBase<T> : IWriter
    {
        protected abstract void Write(XmlWriter writer, T instance);

        void IWriter.Write(XmlWriter writer, object instance) => Write(writer, (T) instance);
    }

    public abstract class WriterBase : IWriter
    {
        public abstract void Write(XmlWriter writer, object instance);
    }

    public interface IWriter
    {
        void Write(XmlWriter writer, object instance);
    }

    public class ValueWriter : ValueWriter<object>
    {
        public ValueWriter(Func<object, string> serialize) : base(serialize) {}
    }

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

        public override void Write(XmlWriter writer, object instance) => _writer.Write(writer, instance);
    }

    public abstract class ElementWriterBase : DecoratedWriter
    {
        protected ElementWriterBase(IWriter writer) : base(writer) {}

        public override void Write(XmlWriter writer, object instance)
        {
            var type = instance.GetType().GetTypeInfo();
            var name = Get(type);
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