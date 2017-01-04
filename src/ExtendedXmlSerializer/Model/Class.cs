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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    public interface ITypeProvider : IParameterizedSource<XElement, Type> {}

    public class HintedRootTypeProvider : ITypeProvider
    {
        private readonly Type _hint;
        private readonly ITypeProvider _provider;

        public HintedRootTypeProvider(Type hint, ITypeProvider provider)
        {
            _hint = hint;
            _provider = provider;
        }

        public Type Get(XElement parameter)
            => parameter.Document.Root == parameter ? _hint : _provider.Get(parameter);
    }

    public class TypeProvider : ITypeProvider
    {
        public static TypeProvider Default { get; } = new TypeProvider();
        TypeProvider() : this(Types.Default, Identities.Default) {}

        private readonly ITypeParser _parser;
        private readonly IIdentities _identities;

        public TypeProvider(ITypeParser parser, IIdentities identities)
        {
            _parser = parser;
            _identities = identities;
        }

        public Type Get(XElement parameter) =>
            _identities.Get(parameter.Name) ?? FromAttribute(parameter);

        private Type FromAttribute(XElement parameter)
        {
            var value = parameter.Attribute(ExtendedXmlSerializer.Type)?.Value;
            var result = value != null ? _parser.Get(value) : null;
            if (result == null)
            {
                throw new SerializationException($"Could not find TypeDefinition from provided value: {value}");
            }

            return result;
        }
    }

    public interface IIdentities : ISpecification<Type>, ISpecification<XName>,
                                   IParameterizedSource<Type, XName>, IParameterizedSource<XName, Type> {}

    public class Identities : IIdentities
    {
        public static Identities Default { get; } = new Identities();
        Identities() : this(string.Empty) {}

        private readonly IDictionary<Type, XName> _names;
        private readonly IDictionary<XName, Type> _types;

        public Identities(string namespaceName)
            : this(new Dictionary<Type, XName>
                   {
                       {typeof(int), XName.Get("int", namespaceName)}
                   }) {}

        public Identities(IDictionary<Type, XName> names)
            : this(names, names.ToDictionary(x => x.Value, y => y.Key)) {}

        public Identities(IDictionary<Type, XName> names, IDictionary<XName, Type> types)
        {
            _names = names;
            _types = types;
        }

        public XName Get(Type parameter)
        {
            XName result;
            return _names.TryGetValue(parameter, out result) ? result : null;
        }

        public Type Get(XName parameter)
        {
            Type result;
            return _types.TryGetValue(parameter, out result) ? result : null;
        }

        public bool IsSatisfiedBy(Type parameter) => _names.ContainsKey(parameter);

        public bool IsSatisfiedBy(XName parameter) => _types.ContainsKey(parameter);
    }

    public interface IConverter : IConditionalWriter, IConditionalReader {}

    public class Converter : ConverterBase
    {
        private readonly ISpecification<TypeInfo> _specification;
        private readonly IWriter _writer;
        private readonly IReader _reader;

        public Converter(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
        {
            _specification = specification;
            _writer = writer;
            _reader = reader;
        }

        public override bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);
        public override void Write(XmlWriter writer, object instance) => _writer.Write(writer, instance);
        public override object Read(XElement element) => _reader.Read(element);
    }

    public abstract class ConverterBase : IConverter
    {
        public abstract bool IsSatisfiedBy(TypeInfo parameter);

        public abstract void Write(XmlWriter writer, object instance);
        public abstract object Read(XElement element);
    }

    public class Primitives : Converter
    {
        public static Primitives Default { get; } = new Primitives();
        Primitives() : this(Identities.Default) {}

        public Primitives(IIdentities identities)
            : this(
                new IntegerConverter(identities),
                new StringConverter(identities)
            ) {}

        public Primitives(params IPrimitiveConverter[] converters)
            : base(
                new ContainsSpecification<TypeInfo>(new HashSet<TypeInfo>(converters.Select(x => x.TargetType.GetTypeInfo()))),
                new ConditionalCompositeWriter(converters), new ConditionalCompositeReader(converters)) {}
    }

    class ConditionalCompositeConverter : IConverter
    {
        private readonly ImmutableArray<IConverter> _converters;
        private readonly IWriter _writer;
        private readonly IReader _reader;

        public ConditionalCompositeConverter(params IConverter[] converters)
            : this(
                converters.ToImmutableArray(), new ConditionalCompositeWriter(converters),
                new ConditionalCompositeReader(converters)) {}

        public ConditionalCompositeConverter(ImmutableArray<IConverter> converters, IWriter writer, IReader reader)
        {
            _converters = converters;
            _writer = writer;
            _reader = reader;
        }

        public bool IsSatisfiedBy(TypeInfo parameter)
        {
            foreach (var converter in _converters)
            {
                if (converter.IsSatisfiedBy(parameter))
                {
                    return true;
                }
            }
            return false;
        }

        public void Write(XmlWriter writer, object instance) => _writer.Write(writer, instance);
        public object Read(XElement element) => _reader.Read(element);
    }

    public interface IPrimitiveConverter : IConverter
    {
        Type TargetType { get; }
    }

    public class StringConverter : PrimitiveConverterBase<string>
    {
        readonly private static Func<string, string> Self = Self<string>.Default.Get;
        public StringConverter(IIdentities provider) : base(provider, Self, Self) {}
    }

    public class IntegerConverter : PrimitiveConverterBase<int>
    {
        public IntegerConverter(IIdentities identities) : base(identities, XmlConvert.ToString, XmlConvert.ToInt32) {}
    }

    public abstract class PrimitiveConverterBase<T> : Converter, IPrimitiveConverter
    {
        readonly private static Type SupportedType = typeof(T);

        protected PrimitiveConverterBase(IIdentities provider, Func<T, string> serialize,
                                         Func<string, T> deserialize)
            : this(
                IsAssignableSpecification<T>.Default,
                new ElementWriter(provider.Get(SupportedType).Accept, new ValueWriter<T>(serialize)),
                new ElementValueReader<T>(deserialize)
            ) {}

        protected PrimitiveConverterBase(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
            : base(specification, writer, reader) {}

        public Type TargetType => SupportedType;
    }

    /*public interface IContext // : ICommand<IEmitter>
    {
        Type DefinedType { get; }
        // IContent Content { get; }
    }

    /*public interface ITypedContext : IContext
    {
        Type DefinedType { get; }
    }#1#

    public interface IRoot : IContext {}

    public interface IMember : IContext
    {
        bool IsWritable { get; }
    }

    public interface IItem : IContext {}

    public interface IDictionaryKey : IContext {}

    public interface IDictionaryValue : IContext {}

    public interface IContent : ICommand<IWriter>
    {
        // Type ContentType { get; }
    }

    public interface IContextContainer : IContent
    {
        IContext Context { get; }
    }

    public interface IProperty : IContextContainer {}

    public interface IReference : IContextContainer {}

    public interface IInstance : IContent
    {
        IEnumerable<IMember> Members { get; }
    }

    public interface IEnumerableInstance : IInstance
    {
        IEnumerable<IItem> Items { get; }
    }

    public interface IPrimitive : IContent
    {
        object Value { get; }
    }

    abstract class ContentBase : IContent
    {
        protected ContentBase(IContext context)
        {
            Context = context;
        }

        protected IContext Context { get; }

        public abstract void Execute(IWriter parameter);
    }

    class Primitive : ContentBase, IPrimitive
    {
        public Primitive(IContext context) : base(context)
        {
        }

        public override void Execute(IWriter parameter)
        {
            
        }
    }


    public interface IWriter
    {
        void Write(IContext context, IContent content);
    }

    public interface IEmitter : ICommand<IContext> {}*/

    /*public interface ITemplate : ISpecification<IContext>
    {
        void Render(IEmitter emitter, IWriter writer, IContext context);
    }

    sealed class LegacyPrimitiveTemplate : LegacyTemplateBase<IPrimitive>
    {
        public static LegacyPrimitiveTemplate Default { get; } = new LegacyPrimitiveTemplate();
        LegacyPrimitiveTemplate() {}

        protected override void Render(IEmitter emitter, IWriter writer, IContext context, IPrimitive content)
            => writer.Emit(content.Value);
    }*/

    /*abstract class LegacyTemplateBase<T> : ITemplate where T : IContent
    {
        public bool IsSatisfiedBy(IContext parameter) => parameter.Content is T;

        public void Render(IEmitter emitter, IWriter writer, IContext context)
        {
            var content = (T) context.Content;

            /*using (writer.New(element))
            {
                if (EmitType(element))
                {
                    writer.Emit(new TypeProperty(element.Content.Type));
                }

                Render(emitter, writer, element, content);
            }#1#
        }

        protected abstract void Render(IEmitter emitter, IWriter writer, IContext context, T content);

        /*protected virtual bool EmitType(IContext context)
        {
            var content = context.Content;
            var defined = context as ITypedContext;
            if (defined != null)
            {
                if (defined.DefinedType != content.ContentType)
                {
                    return (context as IMember)?.IsWritable ?? true;
                }
            }

            return !(content is IEnumerableInstance) && !(content is IPrimitive);
        }#1#
    }*/

/*    public class TemplatedEmitter : IEmitter
    {
        private readonly IWriter _writer;
        private readonly ITemplate[] _templates;

        public TemplatedEmitter(IWriter writer, params ITemplate[] templates)
        {
            _writer = writer;
            _templates = templates;
        }

        public void Execute(IContext parameter)
        {
            foreach (var template in _templates)
            {
                if (template.IsSatisfiedBy(parameter))
                {
                    Render(parameter, template);
                    return;
                }
            }
            throw new SerializationException(
                $"Could not find a template for the context '{parameter}' with content of type '{parameter.Content.ContentType}'.");
        }

        protected virtual void Render(IContext parameter, ITemplate template)
            => template.Render(this, _writer, parameter);
    }*/


    /*public interface IContentSelector : IParameterizedSource<IContext, IContent>
    {
        
    }*/

    /*public class RootBuilder : IRootBuilder
    {
        /*public static RootBuilder Default { get; } = new RootBuilder();
        RootBuilder() : this(PrimaryInstanceFactory.Default) {}#1#

        private readonly IContentSelector _selector;

        public RootBuilder(IContentSelector selector)
        {
            _selector = selector;
        }

/*        public IElement Get(object parameter)
        {
            var descriptor = new Descriptor(parameter);
            var body = _factory.Get(descriptor);
            var result = new Model.Write.Root(body, descriptor.Name);
            return result;
        }#1#

        public IRoot Get(ISource parameter)
        {
            var content = _selector.Get(parameter);
            var result = new Root(content);
            return result;
        }
    }*/

    /*public interface IContentProvider : IParameterizedSource<IContext, IContent> {}

    class ContentProvider {}*/

    /*public class ContentSelector : IContentSelector
    {
        private readonly IContentSelector[] _selectors;

        public ContentSelector(params IContentSelector[] selectors)
        {
            _selectors = selectors;
        }

        public IContent Get(IContext parameter)
        {
            foreach (var selector in _selectors)
            {
                var content = selector.Get(parameter);
                if (content != null)
                {
                    return content;
                }
            }
            // TODO: throw;
            return null;
        }
    }*/


    /*public class PrimitiveContentSelector : IContentSelector
        {
            public static PrimitiveContentSelector Default { get; } = new PrimitiveContentSelector();
            PrimitiveContentSelector() : this(Primitives.Default) {}
    
            private readonly IDictionary<Type, string> _primitives;
    
            public PrimitiveContentSelector(IDictionary<Type, string> primitives)
            {
                _primitives = primitives;
            }
    
            public IContent Get(IContext parameter)
            {
                var type = parameter.DefinedType;
                var result = _primitives.ContainsKey(type) ? new Primitive(parameter) : null;
                return result;
            }
        }*/
}