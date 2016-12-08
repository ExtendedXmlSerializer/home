using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Common
{
    public abstract class InstructionBase<T> : IInstruction where T : IServiceProvider
    {
        public virtual void Execute(IServiceProvider services) => OnExecute(services.AsValid<T>());

        protected abstract void OnExecute(T services);
    }

    class ConditionalInstruction : ConditionalInstruction<IServiceProvider>
    {
        public ConditionalInstruction(ISpecification<IServiceProvider> specification, IInstruction instruction)
            : base(specification, instruction) {}
    }

    class ConditionalInstruction<T> : DecoratedInstruction<T> where T : IServiceProvider
    {
        private readonly ISpecification<T> _specification;
        
        public ConditionalInstruction(ISpecification<T> specification, IInstruction instruction) : base(instruction)
        {
            _specification = specification;
        }

        protected override void OnExecute(T services)
        {
            if (_specification.IsSatisfiedBy(services))
            {
                base.OnExecute(services);
            }
        }
    }

    public class DecoratedInstruction : DecoratedInstruction<IServiceProvider>
    {
        public DecoratedInstruction(IInstruction instruction) : base(instruction) {}
    }

    public class DecoratedInstruction<T> : InstructionBase<T> where T : IServiceProvider
    {
        private readonly IInstruction _instruction;

        public DecoratedInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void OnExecute(T services) => _instruction.Execute(services);
    }

    public class EmptyInstruction : IInstruction
    {
        public static EmptyInstruction Default { get; } = new EmptyInstruction();
        EmptyInstruction() {}

        public void Execute(IServiceProvider services) {}
    }

    public interface IAssignableInstruction : IInstruction
    {
        void Assign(IInstruction instruction);
    }

    public interface IInstruction
    {
        void Execute(IServiceProvider services);
    }

    public abstract class ExtensionBase<T> : IExtension<T> where T : IServiceProvider
    {
        bool IExtension.Starting(IServiceProvider services) => !(services is T) || Starting((T) services);

        public abstract bool Starting(T services);

        void IExtension.Finished(IServiceProvider services)
        {
            if (services is T)
            {
                Finished((T) services);
            }
        }

        public abstract void Finished(T services);
    }

    public interface IExtension<in T> : IExtension where T : IServiceProvider
    {
        bool Starting(T services);
        void Finished(T services);
    }

    public interface IExtension
    {
        bool Starting(IServiceProvider services);
        void Finished(IServiceProvider services);
    }

    public class CompositeServiceProvider : IServiceProvider
    {
        private readonly IEnumerable<IServiceProvider> _providers;
        private readonly IEnumerable<object> _services;

        public CompositeServiceProvider(params object[] services) : this(services.OfType<IServiceProvider>().ToImmutableList(), services) {}
        public CompositeServiceProvider(IEnumerable<IServiceProvider> providers, params object[] services) : this(providers, services.AsEnumerable()) {}

        public CompositeServiceProvider(IEnumerable<object> services) : this(Enumerable.Empty<IServiceProvider>(), services) {}

        public CompositeServiceProvider(IEnumerable<IServiceProvider> providers, IEnumerable<object> services)
        {
            _providers = providers;
            _services = services;
        }

        public object GetService(Type serviceType) => _services.FirstOrDefault(serviceType.GetTypeInfo().IsInstanceOfType) ?? FromServices(serviceType);

        private object FromServices(Type serviceType)
        {
            foreach (var service in _providers)
            {
                var result = service.GetService(serviceType);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }

    public interface IServiceRepository : IServiceProvider
    {
        void Add(object service);
    }

    public interface IPrefixGenerator
    {
        string Generate(Uri identifier);
    }

    class PrefixGenerator : IPrefixGenerator
    {
        private int _count = 1;
        public string Generate(Uri identifier)
        {
            var result = string.Concat("ns", _count++.ToString());
            return result;
        }
    }

    class NamespaceLocator : WeakCacheBase<Type, Uri>, INamespaceLocator
    {
        readonly private static Assembly Assembly = typeof(ExtendedXmlSerializer).GetTypeInfo().Assembly;
        private readonly ISpecification<Type> _primitiveSpecification;
        private readonly Uri _root, _primitive;
        readonly private Assembly _assembly;

        public NamespaceLocator(Uri root) 
            : this(IsPrimitiveSpecification.Default, root, PrimitiveNamespace.Default.Identifier) {}
        public NamespaceLocator(ISpecification<Type> primitiveSpecification, Uri root,
                                Uri primitive)
        {
            _primitiveSpecification = primitiveSpecification;
            _root = root;
            _primitive = primitive;
            _assembly = Assembly;
        }

        public Uri Get(object parameter) => (parameter as INamespace)?.Identifier ?? FromType(parameter);

        private Uri FromType(object parameter)
        {
            var type = parameter as Type ?? parameter.GetType();
            var result = Equals(type.GetTypeInfo().Assembly, _assembly) ? _root : base.Get(type);
            return result;
        }

        protected override Uri Callback(Type key)
        {
            var result =
                _primitiveSpecification.IsSatisfiedBy(key) ? _primitive : new Uri($"clr-namespace:{key.Namespace};assembly={key.GetTypeInfo().Assembly.GetName().Name}");
            return result;
        }
    }

    public interface IRootElement : IElement
    {
        object Root { get; }
    }

    class RootElement : Element, IRootElement
    {
        public RootElement(Uri @namespace, object root) : base(@namespace, TypeDefinitionCache.GetDefinition(root.GetType()).Name)
        {
            Root = root;
        }

        public object Root { get; }
    }

    public interface IElement : INamespace
    {
        string Name { get; }
    }

    public interface INamespace : IUniqueResource
    {
        string Prefix { get; }
    }

    public sealed class PrimitiveNamespace : Namespace
    {
        private new const string Prefix = "sys";

        public new static PrimitiveNamespace Default { get; } = new PrimitiveNamespace();
        PrimitiveNamespace() : base(Prefix, new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/primitives")) {}
    }

    public sealed class RootNamespace : Namespace
    {
        private new const string Prefix = "exs";

        public RootNamespace(Uri identifier) : base(Prefix, identifier) {}
    }

    public class Namespace : INamespace, IEquatable<Namespace>
    {
        public static Namespace Default { get; } = new Namespace();
        Namespace() {}

        public Namespace(Uri identifier) : this(null, identifier) {}

        public Namespace(string prefix, Uri identifier)
        {
            Prefix = prefix;
            Identifier = identifier;
        }

        public string Prefix { get; }
        public Uri Identifier { get; }

        public bool Equals(Namespace other) => 
            !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Equals(Identifier, other.Identifier));

        public override bool Equals(object obj) => !ReferenceEquals(null, obj) &&
                                                   (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Namespace) obj));

        public override int GetHashCode() => Identifier?.GetHashCode() ?? 0;
        public static bool operator ==(Namespace left, Namespace right) => Equals(left, right);
        public static bool operator !=(Namespace left, Namespace right) => !Equals(left, right);
    }

    public interface INamespaceLocator : IParameterizedSource<object, Uri> {}

    public class ServiceRepository : CompositeServiceProvider, IServiceRepository
    {
        private readonly ICollection<object> _services;

        public ServiceRepository(params object[] services) : this(new OrderedSet<object>(services)) {}

        public ServiceRepository(ICollection<object> services) : base(services)
        {
            _services = services;
        }
        public void Add(object service) => _services.Add(service);
    }
    class CompositeExtension : CompositeServiceProvider, IExtension
    {
        private readonly IEnumerable<IExtension> _extensions;

        public CompositeExtension(params IExtension[] extensions) : this(extensions.ToImmutableList()) {}

        public CompositeExtension(ICollection<IExtension> extensions) : base(extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IServiceProvider services)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Starting(services))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IServiceProvider services)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(services);
            }
        }
    }

    abstract class NewContextInstructionBase<T> : DecoratedInstruction<T> where T : IServiceProvider
    {
        protected NewContextInstructionBase(IInstruction instruction) : base(new ExtensionEnabledInstruction(instruction)) {}

        protected override void OnExecute(T services)
        {
            using (DetermineContext(services))
            {
                base.OnExecute(services);
            }
        }

        protected abstract IDisposable DetermineContext(T writing);
    }
}
