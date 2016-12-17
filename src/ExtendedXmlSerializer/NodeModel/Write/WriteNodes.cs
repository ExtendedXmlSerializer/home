using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Sources;
using ExtendedXmlSerialization.Specifications;

namespace ExtendedXmlSerialization.NodeModel.Write
{
    public interface IRoot : IObjectNode<IObjectNode> {}
    public class Root : ObjectNodeBase<IObjectNode>, IRoot
    {
        public Root(IObjectNode body, Type type, string name) : base(body, type, name) {}
    }

    public interface IMembers : IObjectNodeContainer<object> {}
    public class Members : ObjectNodeContainerBase<object>, IMembers
    {
        public Members(Type type, string name, object instance, IEnumerable<IObjectNode> members) : base(instance, type, name, members) {}
    }

    public interface IMember : IObjectNode<IObjectNode>
    {
        IMemberDefinition MemberDefinition { get; }
    }
    public abstract class MemberBase : ObjectNodeBase<IObjectNode>
    {
        protected MemberBase(IObjectNode instance, IMemberDefinition definition)
            : base(instance, definition.Type, definition.Name)
        {
            MemberDefinition = definition;
        }

        public IMemberDefinition MemberDefinition { get; }
    }

    public interface IProperty : IMember {}
    public class Property : MemberBase, IProperty
    {
        public Property(IObjectNode instance, IMemberDefinition definition)
            : base(instance, definition) {}
    }

    public interface IContent : IMember {}
    public class Content : MemberBase, IContent
    {
        public Content(IObjectNode instance, IMemberDefinition definition)
            : base(instance, definition) {}
    }

    public interface IInstance : IObjectNode
    {
        long Id { get; }

        IMembers Members { get; }
    }

    public class Instance : Instance<object>
    {
        public Instance(long id, object instance, Type type, string name, IMembers members)
            : base(id, instance, type, name, members) {}
    }

    public class Instance<T> : ObjectNodeBase<T>, IInstance
    {
        public Instance(long id, T instance, Type type, string name, IMembers members)
        : base(instance, type, name)
        {
            Id = id;
            Members = members;
        }

        public long Id { get; }
        public IMembers Members { get; }
    }

    public abstract class ObjectNodeContainerBase<T> : ObjectNodeBase<T>, IObjectNodeContainer<T>
    {
        private readonly IEnumerable<IObjectNode> _nodes;

        protected ObjectNodeContainerBase(T instance, Type type, string name, IEnumerable<IObjectNode> nodes)
            : base(instance, type, name)
        {
            _nodes = nodes;
        }

        public IEnumerator<IObjectNode> GetEnumerator() => _nodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    public interface IPrimitive : IObjectNode {}
    public class Primitive : ObjectNodeBase<object>, IPrimitive
    {
        public Primitive(object instance) : this(instance, instance.GetType()) {}
        public Primitive(object instance, Type type) : this(instance, type, type.Name) {}
        public Primitive(object instance, Type type, string name) : base(instance, type, name) {}
    }

    public interface IReference : IObjectNode {}
    public class Reference : ObjectNodeBase<IInstance>, IReference
    {
        public Reference(IInstance instance) : this(instance, instance.Type, instance.Name) {}
        public Reference(IInstance instance, Type type, string name) : base(instance, type, name) {}
    }

    public interface IEnumerableInstance : IObjectNode<IEnumerable>, IInstance
    {
        Type ElementType { get; }
    }

    public class EnumerableInstance : EnumerableInstanceBase<IEnumerable>, IEnumerableInstance
    {
        public EnumerableInstance(long id, Type elementType,
                                  IEnumerable instance, Type type, string name, IEnumerable<IMember> members, IEnumerable<IObjectNode> elements)
            : base(id, elementType, instance, type, name, members, elements) {}
    }

    public abstract class EnumerableInstanceBase<T> : Instance<T> where T : IEnumerable
    {
        protected EnumerableInstanceBase(long id, Type elementType,
                                         T instance, Type type, string name, IEnumerable<IMember> members, IEnumerable<IObjectNode> entries)
            : base(id, instance, type, name, new Members(type, name, instance, members.Concat(entries).ToArray()))
        {
            ElementType = elementType;
        }

        public Type ElementType { get; }
    }

    public interface IDictionaryInstance : IObjectNode {}

    public class DictionaryInstance : EnumerableInstanceBase<IDictionary>, IDictionaryInstance
    {
        public DictionaryInstance(long id, IDictionary instance, Type type, string name, IEnumerable<IMember> members,
                                  IEnumerable<IDictionaryEntry> entries)
            : base(id, DictionaryEntryInstance.DefaultElementType, instance, type, name, members, entries) {}
    }

    public interface IDictionaryEntry : IObjectNodeContainer<DictionaryEntry> {}

    public class DictionaryEntryInstance : ObjectNodeContainerBase<DictionaryEntry>, IDictionaryEntry
    {
        public static Type DefaultElementType { get; } = typeof(DictionaryEntry);

        public DictionaryEntryInstance(DictionaryEntry instance,
                                       IDictionaryKey key, IDictionaryValue value)
            : base(instance, DefaultElementType, ExtendedXmlSerializer.Item, key.Append<IObjectNode>(value)) {}
    }

    public interface IDictionaryKey : IObjectNode<IObjectNode> {}

    public class DictionaryKey : ObjectNodeBase<IObjectNode>, IDictionaryKey
    {
        public DictionaryKey(IObjectNode instance) : this(instance, instance.Type) {}
        public DictionaryKey(IObjectNode instance, Type type) : base(instance, type, ExtendedXmlSerializer.Key) {}
    }

    public interface IDictionaryValue : IObjectNode<IObjectNode> {}

    public class DictionaryValue : ObjectNodeBase<IObjectNode>, IDictionaryValue
    {
        public DictionaryValue(IObjectNode instance) : this(instance, instance.Type) {}
        public DictionaryValue(IObjectNode instance, Type type) : base(instance, type, ExtendedXmlSerializer.Value) {}
    }

    public abstract class ObjectNodeBase<T> : NodeBase, IObjectNode<T>
    {
        protected ObjectNodeBase(T instance, Type type, string name) : base(name)
        {
            Instance = instance;
            Type = type;
        }

        public T Instance { get; }
        object IObjectNode.Instance => Instance;

        public Type Type { get; }
    }

    public interface IObjectNodeContainer<out T> : IObjectNode<T>, INodeContainer<IObjectNode> {}

    public interface IQualifiedNode : INode
    {
        Type Type { get; }
    }

    public interface IObjectNode : IQualifiedNode
    {
        object Instance { get; }
    }

    public interface IObjectNode<out T> : IObjectNode
    {
        new T Instance { get; }
    }

    public interface INodeBuilder
    {
        IObjectNode Create();
    }

/*
    class NodeBuilder : INodeBuilder
    {
        readonly private static Func<IObjectNode, bool> Property = NeverSpecification<IObjectNode>.Default.IsSatisfiedBy;

        readonly private IDictionary<long, IInstance> _instances = new Dictionary<long, IInstance>();
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();
        private readonly Func<IObjectNode, bool> _property;
        private readonly object _instance;
        private readonly ITypeDefinition _definition;
        readonly private Func<DictionaryEntry, IDictionaryEntry> _createEntry;

        public NodeBuilder(object instance) : this(Property, instance) {}

        public NodeBuilder(Func<IObjectNode, bool> property, object instance)
            : this(property, instance, TypeDefinitionCache.GetDefinition(instance.GetType())) {}

        public NodeBuilder(Func<IObjectNode, bool> property, object instance, ITypeDefinition definition)
        {
            _property = property;
            _instance = instance;
            _definition = definition;
            _createEntry = CreateEntry;
        }

        public IObjectNode Create() => Select(_instance, _definition);

        private IObjectNode Select(object instance) =>
            Select(instance, TypeDefinitionCache.GetDefinition(instance.GetType()));

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
            var members = CreateMembers(instance, ensured).ToArray();
            
            var dictionary = instance as IDictionary;
            if (dictionary != null)
            {
                return new DictionaryInstance(id, dictionary, definition.Type, definition.Name, members,
                                              dictionary.OfType<DictionaryEntry>().Select(_createEntry));
            }
            if (IsEnumerableTypeSpecification.Default.IsSatisfiedBy(ensured))
            {
                return new EnumerableInstance(id, definition.GenericArguments[0], (IEnumerable) instance, definition.Type, definition.Name,
                                              members,
                                              CreateElements(instance));
            }
            var result = new Instance(id, instance, definition.Type, definition.Name, new Members(definition.Type, definition.Name instance, members));
            return result;
        }

        IEnumerable<IMember> CreateMembers(object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                var node = Select(member.GetValue(instance), member.TypeDefinition);
                var item = _property(node) ? (IMember) new Property(node, member) : new Content(node, member);
                yield return item;
            }
        }

        private IEnumerable<IObjectNode> CreateElements(object instance)
        {
            foreach (var element in Arrays.Default.AsArray(instance))
            {
                yield return Select(element);
            }
        }

        private IDictionaryEntry CreateEntry(DictionaryEntry entry)
        {
            var key = new DictionaryKey(Select(entry.Key));
            var value = new DictionaryValue(Select(entry.Value));
            var result = new DictionaryEntryInstance(entry, key, value);
            return result;
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
    }
*/

    /*class RootNodeBuilder : IParameterizedSource<object, IRoot>
    {
        public static RootNodeBuilder Default { get; } = new RootNodeBuilder();
        RootNodeBuilder() {}

        public IRoot Get(object parameter)
        {
            var type = parameter.GetType();
            var definition = TypeDefinitionCache.GetDefinition(type);
            var result = new Root(new NodeBuilder(parameter).Create(), type, definition.Name);
            return result;
        }
    }*/
}