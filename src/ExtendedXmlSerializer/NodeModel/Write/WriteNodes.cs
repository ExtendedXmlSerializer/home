using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.ProcessModel;
using ExtendedXmlSerialization.ProcessModel.Write;
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

    /*public interface IMember : IObjectNode<IObjectNode>
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
    }*/

    public interface IInstance : IObjectNode
    {
        long Id { get; }

        IMembers Members { get; }
    }

    public class Instance : Instance<object>
    {
        public Instance(long id, object instance, Type type, string name, IEnumerable<IObjectNode> members)
            : this(id, instance, type, name, new Members(type, name, instance, members)) {}

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

    public interface IProperty : IObjectNode {}
    public class Property : PropertyBase<object>
    {
        public Property(object instance, Type type, string name) : base(instance, type, name) {}
    }

    public abstract class PropertyBase<T> : ObjectNodeBase<T>, IProperty
    {
        protected PropertyBase(T instance, Type type, string name) : base(instance, type, name) {}
    }

    public class TypeProperty : PropertyBase<Type>
    {
        public TypeProperty(Type type) : base(type, typeof(IExtendedXmlSerializer), ExtendedXmlSerializer.Type) {}
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
                                  IEnumerable instance, Type type, string name, IEnumerable<IObjectNode> nodes)
            : base(id, elementType, instance, type, name, nodes) {}
    }

    public abstract class EnumerableInstanceBase<T> : Instance<T> where T : IEnumerable
    {
        protected EnumerableInstanceBase(long id, Type elementType,
                                         T instance, Type type, string name, IEnumerable<IObjectNode> nodes)
            : base(id, instance, type, name, new Members(type, name, instance, nodes))
        {
            ElementType = elementType;
        }

        public Type ElementType { get; }
    }

    public interface IDictionaryInstance : IObjectNode {}

    public class DictionaryInstance : EnumerableInstanceBase<IDictionary>, IDictionaryInstance
    {
        public DictionaryInstance(long id, IDictionary instance, Type type, string name, IEnumerable<IObjectNode> nodes)
            : base(id, DictionaryEntryInstance.DefaultElementType, instance, type, name, nodes) {}
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

    public interface IRootNodeBuilder : IParameterizedSource<object, IObjectNode> {}

    class DefaultRootNodeBuilder : IRootNodeBuilder
    {
        readonly private static Func<ITypeDefinition, bool> IsEnumerable =
            IsEnumerableTypeSpecification.Default.IsSatisfiedBy;

        readonly private static Func<Type, TypeDefinition> Definition = TypeDefinitionCache.GetDefinition;

        readonly private IDictionary<long, IInstance> _instances = new Dictionary<long, IInstance>();
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public IObjectNode Get(object parameter)
        {
            var type = parameter.GetType();
            var definition = Definition(type);
            var result = Select(parameter, definition);
            return result;
        }

        IObjectNode Select(object instance, IMemberDefinition member) => Select(instance, member, member.TypeDefinition);
        private IObjectNode Select(object instance, ITypeDefinition type) => Select(instance, type, type);

        IObjectNode Select(object instance, IDefinition source, ITypeDefinition type)
        {
            if (type.IsPrimitive)
            {
                return new Primitive(instance, source.Type, source.Name);
            }

            bool first;
            var id = _generator.GetId(instance, out first);
            if (first)
            {
                var @select = _instances[id] = GetInstance(id, instance, source, type);
                return @select;
            }

            if (_instances.ContainsKey(id))
            {
                return new Reference(_instances[id]);
            }
            throw new InvalidOperationException(
                      $"Could not create a context for instance '{instance}' of type '{type.Type}'.");
        }

        private IInstance GetInstance(long id, object instance, IDefinition source, ITypeDefinition type)
        {
            var ensured = type.For(instance);
            var members = CreateMembers(instance, ensured) /*.OrderBy(x => x is IProperty)*/;

            var dictionary = instance as IDictionary;
            if (dictionary != null)
            {
                var key = Definition(type.GenericArguments[0]);
                var value = Definition(type.GenericArguments[1]);
                var entries = CreateEntries(dictionary, key, value);
                return new DictionaryInstance(id, dictionary, source.Type, source.Name, members.Concat(entries));
            }

            if (IsEnumerable(ensured))
            {
                var elementType = Definition(type.GenericArguments[0]);
                var elements = CreateElements(instance, elementType);
                return new EnumerableInstance(id, elementType.Type, (IEnumerable) instance, source.Type,
                                              source.Name,
                                              members.Concat(elements));
            }
            var result = new Instance(id, instance, source.Type, source.Name, members);
            return result;
        }

        IEnumerable<IObjectNode> CreateMembers(object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                yield return Select(member.GetValue(instance), member);
            }
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
    }
}