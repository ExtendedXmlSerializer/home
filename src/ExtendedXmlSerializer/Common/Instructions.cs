using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Common
{
    public abstract class InstructionBase<T> : IInstruction where T : IServiceProvider
    {
        public virtual void Execute(IServiceProvider services) => Execute(services.AsValid<T>());

        protected abstract void Execute(T services);
    }

    public class DecoratedInstruction<T> : InstructionBase<T> where T : IServiceProvider
    {
        private readonly IInstruction _instruction;

        public DecoratedInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        protected override void Execute(T services) => _instruction.Execute(services);
    }

    public class EmptyInstruction : IInstruction
    {
        public static EmptyInstruction Default { get; } = new EmptyInstruction();
        EmptyInstruction() {}

        public void Execute(IServiceProvider services) {}
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

    class NamespaceLocator : WeakCacheBase<Type, INamespace>, INamespaceLocator
    {
        private const string Prefix = "exs";
        private readonly INamespace _root;
        readonly private Assembly _assembly;

        public NamespaceLocator(Uri root) : this(Prefix, root) {}

        public NamespaceLocator(string prefix, Uri root) : this(new Namespace(prefix, root)) {}

        public NamespaceLocator(INamespace root)
        {
            _root = root;
            _assembly = GetType().GetTypeInfo().Assembly;
        }

        public INamespace Get(object parameter) => parameter as INamespace ?? FromType(parameter);

        private INamespace FromType(object parameter)
        {
            var type = parameter as Type ?? parameter.GetType();
            var result = Equals(type.GetTypeInfo().Assembly, _assembly) ? _root : base.Get(type);
            return result;
        }

        protected override INamespace Callback(Type key) => new Namespace(new Uri($"clr-namespace:{key.Namespace};assembly={key.GetTypeInfo().Assembly.GetName().Name}"));
    }

    class DefaultNamespaceLocator : INamespaceLocator
    {
        public static DefaultNamespaceLocator Default { get; } = new DefaultNamespaceLocator();
        DefaultNamespaceLocator() {}

        public INamespace Get(object parameter) => null;
    }

    public interface IElement : INamespace
    {
        string Name { get; }
    }

    public interface INamespace : IUniqueResource
    {
        string Prefix { get; }
    }

    public class Namespace : INamespace
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
    }

    public interface INamespaceLocator : IParameterizedSource<object, INamespace> {}

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
        protected NewContextInstructionBase(IInstruction instruction) : base(instruction) {}

        protected override void Execute(T services)
        {
            using (DetermineContext(services))
            {
                var extension = services.Get<IExtension>();
                if (extension != null)
                {
                    if (extension.Starting(services))
                    {
                        base.Execute(services);
                    }
                    extension.Finished(services);
                }
                else
                {
                    base.Execute(services);
                }
            }
        }

        protected abstract IDisposable DetermineContext(T writing);
    }
}
