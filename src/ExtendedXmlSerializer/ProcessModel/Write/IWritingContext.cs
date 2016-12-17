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
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.NodeModel.Write;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Sources;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.ProcessModel.Write
{
    public interface ISerialization : ICommand<object>, IDisposable // : IWriter, INodeEmitter //  IProcess
    {}

    class Serialization : ISerialization
    {
        readonly private static Func<IObjectNode, bool> Property = NeverSpecification<IObjectNode>.Default.IsSatisfiedBy;

        readonly private IDictionary<long, IInstance> _instances = new Dictionary<long, IInstance>();
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();
        private readonly Func<IObjectNode, bool> _property;
        private readonly IWriter _writer;

        public Serialization(IWriter writer) : this(writer, Property) {}

        public Serialization(IWriter writer, Func<IObjectNode, bool> property)
        {
            _writer = writer;
            _property = property;
        }

        public void Execute(object parameter)
        {
            var type = parameter.GetType();
            var definition = TypeDefinitionCache.GetDefinition(type);
            var body = Select(parameter, definition);
            var root = new Root(body, definition.Type, definition.Name);
            Process(root);
        }

        void Process(IObjectNode node)
        {
            var primitive = node as IPrimitive;
            if (primitive != null)
            {
                _writer.Emit(primitive.Instance);
                return;
            }

            var container = node as IEnumerable<IObjectNode>;
            if (container != null)
            {
                foreach (var item in container)
                {
                    Process(item);
                }
                return;
            }

            var property = node as IProperty;
            if (property != null)
            {
                _writer.Emit(property);
                return;
            }

            var content = node as IContent;
            if (content != null)
            {
                using (_writer.Begin(content))
                {
                    /*if (content.MemberDefinition.IsWritable)
                    {
                        var type = content.Instance.Instance.GetType();
                        if (content.Type != type)
                        {
                            
                        }
                        _writer.Emit(new TypeProperty(type));
                    }*/
                    Process(content.Instance);
                }
                return;
            }

            var enumerable = node as IEnumerableInstance;
            if (enumerable != null)
            {
                using (_writer.Begin(enumerable))
                {
                    Process(enumerable.Members);
                }
                return;
            }

            var instance = node as IInstance;
            if (instance != null)
            {
                using (_writer.Begin(instance))
                {
                    _writer.Emit(new TypeProperty(instance.Instance.GetType()));
                    Process(instance.Members);
                }
                return;
            }

            var root = node as IRoot;
            if (root != null)
            {
                Process(root.Instance);
            }
        }

        IObjectNode Select(object instance, ITypeDefinition definition)
        {
            if (definition.IsPrimitive)
            {
                return new Primitive(instance, definition.Type, definition.Name);
            }

            bool first;
            var id = _generator.GetId(instance, out first);
            if (first)
            {
                var @select = _instances[id] = GetInstance(id, instance, definition);
                return @select;
            }

            if (_instances.ContainsKey(id))
            {
                return new Reference(_instances[id]);
            }
            throw new InvalidOperationException(
                      $"Could not create a context for instance '{instance}' of type '{definition.Type}'.");
        }

        private IInstance GetInstance(long id, object instance, ITypeDefinition definition)
        {
            var ensured = EnsureDefinition(definition, instance);
            var members = CreateMembers(instance, ensured);

            var dictionary = instance as IDictionary;
            if (dictionary != null)
            {
                var key = TypeDefinitionCache.GetDefinition(definition.GenericArguments[0]);
                var value = TypeDefinitionCache.GetDefinition(definition.GenericArguments[1]);
                return new DictionaryInstance(id, dictionary, definition.Type, definition.Name, members,
                                              CreateEntries(dictionary, key, value));
            }
            if (IsEnumerableTypeSpecification.Default.IsSatisfiedBy(ensured))
            {
                var elementType = TypeDefinitionCache.GetDefinition(definition.GenericArguments[0]);
                return new EnumerableInstance(id, elementType.Type, (IEnumerable) instance, definition.Type,
                                              definition.Name,
                                              members,
                                              CreateElements(instance, elementType));
            }
            var result = new Instance(id, instance, definition.Type, definition.Name, new Members(instance, members));
            return result;
        }

        IEnumerable<IMember> CreateMembers(object instance, ITypeDefinition definition)
        {
            /*var properties = new List<IMember>(definition.Members.Count);
            var contents = new List<IMember>(definition.Members.Count);*/
            foreach (var member in definition.Members)
            {
                var node = Select(member.GetValue(instance), member.TypeDefinition);
                /*if (_property(node))
                {
                    properties.Add(new Property(node, member));
                }
                else
                {
                    contents.Add(new Content(node, member));
                }*/
                yield return _property(node) ? (IMember) new Property(node, member) : new Content(node, member);
            }
            /*var result = properties.Concat(contents);
            return result;*/
        }

        private IEnumerable<IObjectNode> CreateElements(object instance, ITypeDefinition elementType)
        {
            foreach (var element in Arrays.Default.AsArray(instance))
            {
                yield return Select(element, elementType);
            }
        }

        private IEnumerable<IDictionaryEntry> CreateEntries(IDictionary dictionary, ITypeDefinition keyDefinition,
                                                            ITypeDefinition valueDefinition)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = new DictionaryKey(Select(entry.Key, keyDefinition));
                var value = new DictionaryValue(Select(entry.Value, valueDefinition));
                var result = new DictionaryEntryInstance(entry, key, value);
                yield return result;
            }
        }

        private static ITypeDefinition EnsureDefinition(ITypeDefinition definition, object value)
        {
            if (!Equals(value, definition.DefaultValue))
            {
                var type = value.GetType();
                if (type != definition.Type)
                {
                    return TypeDefinitionCache.GetDefinition(type);
                }
            }
            return definition;
        }

        public void Dispose() => _writer.Dispose();
    }

    /*class Members : IEnumerable<IElement>
    {
        private readonly ISelector _selector;
        private readonly object _instance;
        private readonly ITypeDefinition _definition;

        public Members(ISelector selector, object instance) : this(selector, instance, TypeDefinitionCache.GetDefinition(instance.GetType())) {}

        public Members(ISelector selector, object instance, ITypeDefinition definition)
        {
            _selector = selector;
            _instance = instance;
            _definition = definition;
        }

        public IEnumerator<IElement> GetEnumerator()
        {
            foreach (var member in _definition.Members)
            {
                yield return _selector.Get(member.GetValue(_instance));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Context : Element, IContext
    {
        private readonly IEnumerable<IElement> _elements;
        
        public Context(IDefinition definition, IEnumerable<IElement> elements) : base(definition)
        {
            _elements = elements;
        }

        public IEnumerator<IElement> GetEnumerator() => _elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal interface ISelector : IParameterizedSource<object, IElement> {}

    public interface IRoot : IContext
    {
        object Root { get; }
    }

    class Root : Context, IRoot {
        private readonly object _root;

        public Root(object root, ITypeDefinition definition, IEnumerable<IElement> elements) : base(definition, elements)
        {
            _root = root;
        }

        object IRoot.Root => _root;
    }

    public interface IMembers : IContext
    {
        object Instance { get; }
    }

    public interface IContext : IElement, IEnumerable<IElement> {}

    public interface IValue : IElement
    {
        object Value { get; }
    }

    public class Element : IElement
    {
        private readonly Type _type;
        private readonly string _name;
        
        public Element(IDefinition definition) : this(definition.Type, definition.Name) {}

        public Element(Type type, string name)
        {
            _type = type;
            _name = name;
        }

        public Type Type { get; }
        public string Name { get; }
    }

    public interface IElement : IDefinition {}*/
}