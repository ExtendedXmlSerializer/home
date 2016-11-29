using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.Write
{
    public interface ISerializer
    {
        void Serialize(IWriter writer, object instance);
    }

    public class Serializer : ISerializer
    {
        readonly private static IInstructionSource DefaultSource = RootInstructionProviderFactory.Default.Create();
        public static Serializer Default { get; } = new Serializer();
        Serializer() {}

        private readonly IInstructions _instructions;

        Serializer(IInstructions instructions)
        {
            _instructions = instructions;
        }

        public void Serialize(IWriter writer, object instance)
        {
            var instruction = _instructions.For(instance);
            instruction?.Execute(writer, instance);
        }
    }

    public interface IInstructions
    {
        IInstruction For(object instance);
    }

    interface IInstructionSource
    {
        IInstruction Get(Type type);
    }

    class Instructions : IInstructions
    {
        public IInstruction For(object instance)
        {
            return null;
        }
    }

    class ConditionalInstructionSource : IInstructionSource
    {
        private readonly Func<TypeDefinition, bool> _specification;
        private readonly IInstructionSource _inner;

        public ConditionalInstructionSource(Func<TypeDefinition, bool> specification, IInstructionSource inner)
        {
            _specification = specification;
            _inner = inner;
        }

        public IInstruction Get(Type type) => _specification(TypeDefinitionCache.GetDefinition(type)) ? _inner.Get(type) : null;
    }

    class CanSerializeSpecification
    {
        public static CanSerializeSpecification Default { get; } = new CanSerializeSpecification();
        CanSerializeSpecification() {}

        public bool IsSatisfiedBy(TypeDefinition definition)
            =>
            definition.IsPrimitive || 
            definition.IsDictionary || 
            definition.IsArray || 
            definition.IsEnumerable ||
            definition.IsObjectToSerialize;
    }

    class ObjectInstructionSource : IInstructionSource
    {
        private readonly Func<TypeDefinition, bool> _specification;
        private readonly IInstructionSource _source;

        public ObjectInstructionSource(IInstructionSource source) : this(CanSerializeSpecification.Default.IsSatisfiedBy, source) {}

        public ObjectInstructionSource(Func<TypeDefinition, bool> specification, IInstructionSource source)
        {
            _specification = specification;
            _source = source;
        }

        public IInstruction Get(Type type) => new CompositeInstruction(GetContentInstructions(type));

        IEnumerable<IInstruction> GetContentInstructions(Type type)
        {
            yield return new EmitTypeInstruction(type);
            foreach (var member in TypeDefinitionCache.GetDefinition(type).Properties)
            {
                if (_specification(member.TypeDefinition))
                {
                    /*yield return
                            new EmitMemberObjectInstruction(member,
                                                      new DeferredInstruction(
                                                          new FixedInstruction(_source, member.TypeDefinition.Type).Get));*/
                }
            }
        }
    }

    class RootInstructionProviderFactory
    {
        public static RootInstructionProviderFactory Default { get; } = new RootInstructionProviderFactory();
        RootInstructionProviderFactory() {}

        public IInstructionSource Create()
        {
            var collection = new List<IInstructionSource>();
            var composite = new CachedInstructionSource(new FirstAssignedInstructionSource(collection));
            collection.AddRange(new IInstructionSource[]
                                {
                                    new ConditionalInstructionSource(definition => definition.IsPrimitive, ContentInstructionSource.Default),
                                    new ConditionalInstructionSource(definition => definition.IsDictionary, new DictionaryInstructionSource(composite)),
                                    new ConditionalInstructionSource(definition => definition.IsArray || definition.IsEnumerable,
                                                          new EnumerableInstructionSource(composite)),
                                    new ConditionalInstructionSource(definition => definition.IsObjectToSerialize, new ObjectInstructionSource(composite))
                                });
            var result = new CachedInstructionSource(new RootInstructionSource(composite));
            return result;
        }
    }

    class RootInstructionSource : IInstructionSource
    {
        private readonly IInstructionSource _inner;

        public RootInstructionSource(IInstructionSource inner)
        {
            _inner = inner;
        }

        public IInstruction Get(Type type)
        {
            var content = _inner.Get(type);
            if (content == null)
            {
                throw new InvalidOperationException($"Could not find instruction for type '{type}'");
            }
            var result = new EmitObjectInstruction(type.Name, content);
            return result;
        }
    }

    class CachedInstructionSource : IInstructionSource
    {
        private readonly ConditionalWeakTable<Type, IInstruction> _cache =
            new ConditionalWeakTable<Type, IInstruction>();

        private readonly ConditionalWeakTable<Type, IInstruction>.CreateValueCallback _callback;

        public CachedInstructionSource(IInstructionSource inner) : this(inner.Get) {}

        public CachedInstructionSource(ConditionalWeakTable<Type, IInstruction>.CreateValueCallback callback)
        {
            _callback = callback;
        }

        public IInstruction Get(Type type) => _cache.GetValue(type, _callback);
    }

    class FirstAssignedInstructionSource : IInstructionSource
    {
        readonly ICollection<IInstructionSource> _providers;

        public FirstAssignedInstructionSource(ICollection<IInstructionSource> providers)
        {
            _providers = providers;
        }

        public IInstruction Get(Type type)
        {
            foreach (var provider in _providers)
            {
                var instruction = provider.Get(type);
                if (instruction != null)
                {
                    return instruction;
                }
            }
            return null;
        }
    }

    class EnumerableInstructionSource : IInstructionSource
    {
        private readonly IInstructionSource _source;

        public EnumerableInstructionSource(IInstructionSource source)
        {
            _source = source;
        }

        public IInstruction Get(Type type)
        {
            var elementType = ElementTypeLocator.Default.Locate(type);
            var instructions = _source.Get(elementType);
            var result = new EmitEnumerableInstruction(instructions);
            return result;
        }
    }

    class ContentInstructionSource : IInstructionSource
    {
        public static ContentInstructionSource Default { get; } = new ContentInstructionSource();
        ContentInstructionSource() {}

        public IInstruction Get(Type type) => EmitContentInstruction.Default;
    }

    class DictionaryInstructionSource : IInstructionSource
    {
        private readonly IInstructionSource _source;
        
        public DictionaryInstructionSource(IInstructionSource source)
        {
            _source = source;
        }

        public IInstruction Get(Type type)
        {
            var arguments = TypeDefinitionCache.GetDefinition(type).GenericArguments;
            if (arguments.Length != 2)
            {
                throw new InvalidOperationException(
                          $"Attempted to write type '{type}' as a dictionary, but it does not have enough generic arguments.");
            }
            var keys = new EmitObjectInstruction(ExtendedXmlSerializer.Key, _source.Get(arguments[0]));
            var values = new EmitObjectInstruction(ExtendedXmlSerializer.Value, _source.Get(arguments[1]));
            var result = new EmitDictionaryInstruction(keys, values);
            return result;
        }
    }
}