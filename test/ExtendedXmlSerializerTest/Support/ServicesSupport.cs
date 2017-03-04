using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.ExtensionModel;

namespace ExtendedXmlSerialization.Test.Support
{
	class ServicesSupport : IServices
	{
		readonly static ISerializerExtension[] Empty = {};

		readonly IServices _services;

		public ServicesSupport() : this(Empty) {}

		public ServicesSupport(params ISerializerExtension[] extensions) : this(extensions, new Instances(Activation.Default, new MemberConfiguration(), WellKnownConverters.Default,
		                                                                                                  new XmlFactory(new XmlReaderSettings(), new XmlWriterSettings())).ToArray()) {}
		public ServicesSupport(IEnumerable<ISerializerExtension> extensions, params object[] services) : this(new ConfiguredServices(services).Get(extensions)) {}

		public ServicesSupport(IServices services)
		{
			_services = services;
		}

		public void Dispose() => _services.Dispose();

		public IEnumerable<TypeInfo> AvailableServices => _services.AvailableServices;

		public IServiceRepository Register(Type serviceType, Type implementingType) => _services.Register(serviceType, implementingType);

		public IServiceRepository Register(Type serviceType, Type implementingType, string serviceName) => _services.Register(serviceType, implementingType, serviceName);

		public IServiceRepository Register<TService, TImplementation>() where TImplementation : TService => _services.Register<TService, TImplementation>();

		public IServiceRepository Register<TService, TImplementation>(string serviceName) where TImplementation : TService => _services.Register<TService, TImplementation>(serviceName);

		public IServiceRepository Register<TService>() => _services.Register<TService>();

		public IServiceRepository RegisterInstance<TService>(TService instance) => _services.RegisterInstance(instance);

		public IServiceRepository RegisterInstance<TService>(TService instance, string serviceName) => _services.RegisterInstance(instance, serviceName);

		public IServiceRepository RegisterInstance(Type serviceType, object instance) => _services.RegisterInstance(serviceType, instance);

		public IServiceRepository RegisterInstance(Type serviceType, object instance, string serviceName) => _services.RegisterInstance(serviceType, instance, serviceName);

		public IServiceRepository Register(Type serviceType) => _services.Register(serviceType);
		public IServiceRepository Register<TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, TService> factory) => _services.Register(factory);

		public IServiceRepository Register<T, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T, TService> factory) => _services.Register(factory);

		public IServiceRepository Register<T, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServiceRepository Register<T1, T2, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T1, T2, TService> factory) => _services.Register(factory);

		public IServiceRepository Register<T1, T2, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T1, T2, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServiceRepository Register<T1, T2, T3, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T1, T2, T3, TService> factory) => _services.Register(factory);

		public IServiceRepository Register<T1, T2, T3, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T1, T2, T3, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServiceRepository Register<T1, T2, T3, T4, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T1, T2, T3, T4, TService> factory) => _services.Register(factory);

		public IServiceRepository Register<T1, T2, T3, T4, TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, T1, T2, T3, T4, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServiceRepository Register<TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServiceRepository RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory) => _services.RegisterFallback(predicate, factory);
		public IServiceRepository RegisterConstructorDependency<TDependency>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, ParameterInfo, TDependency> factory) => _services.RegisterConstructorDependency(factory);

		public IServiceRepository RegisterConstructorDependency<TDependency>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, ParameterInfo, object[], TDependency> factory) => _services.RegisterConstructorDependency(factory);

		public IServiceRepository Decorate(Type serviceType, Type decoratorType) => _services.Decorate(serviceType, decoratorType);

		public IServiceRepository Decorate<TService, TDecorator>() where TDecorator : TService => _services.Decorate<TService, TDecorator>();
		public IServiceRepository Decorate<TService>(Func<ExtendedXmlSerialization.ExtensionModel.IServiceProvider, TService, TService> factory) => _services.Decorate(factory);

		public object GetInstance(Type serviceType) => _services.GetInstance(serviceType);

		public object GetInstance(Type serviceType, object[] arguments) => _services.GetInstance(serviceType, arguments);

		public object GetInstance(Type serviceType, string serviceName, object[] arguments) => _services.GetInstance(serviceType, serviceName, arguments);

		public object GetInstance(Type serviceType, string serviceName) => _services.GetInstance(serviceType, serviceName);

		public object TryGetInstance(Type serviceType) => _services.TryGetInstance(serviceType);

		public object TryGetInstance(Type serviceType, string serviceName) => _services.TryGetInstance(serviceType, serviceName);

		public IEnumerable<object> GetAllInstances(Type serviceType) => _services.GetAllInstances(serviceType);

		public object Create(Type serviceType) => _services.Create(serviceType);
		public object GetService(Type serviceType) => _services.GetService(serviceType);
	}
}