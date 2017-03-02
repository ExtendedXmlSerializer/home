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
using LightInject;

namespace ExtendedXmlSerialization.Test.Support
{
	class ServicesSupport : IServices
	{
		readonly static ISerializerExtension[] Empty = {};

		readonly IServices _services;

		public ServicesSupport() : this(Empty) {}

		public ServicesSupport(params ISerializerExtension[] extensions) : this(extensions, new Instances(Activation.Default, new MemberConfiguration(),
		                                                                                                  ContentSource.Default.Get(WellKnownConverters.Default),
		                                                                                                  new XmlFactory(new XmlReaderSettings(), new XmlWriterSettings())).ToArray()) {}
		public ServicesSupport(IEnumerable<ISerializerExtension> extensions, params object[] services) : this(new ConfiguredServices(services).Get(extensions)) {}

		public ServicesSupport(IServices services)
		{
			_services = services;
		}

		public object GetService(Type serviceType) => _services.GetService(serviceType);

		public void Dispose() => _services.Dispose();

		public IEnumerable<ServiceRegistration> AvailableServices => _services.AvailableServices;

		public IServices Register(Type serviceType, Type implementingType) => _services.Register(serviceType, implementingType);

		public IServices Register(Type serviceType, Type implementingType, string serviceName) => _services.Register(serviceType, implementingType, serviceName);

		public IServices Register<TService, TImplementation>() where TImplementation : TService => _services.Register<TService, TImplementation>();

		public IServices Register<TService, TImplementation>(string serviceName) where TImplementation : TService => _services.Register<TService, TImplementation>(serviceName);

		public IServices Register<TService>() => _services.Register<TService>();

		public IServices RegisterInstance<TService>(TService instance) => _services.RegisterInstance(instance);

		public IServices RegisterInstance<TService>(TService instance, string serviceName) => _services.RegisterInstance(instance, serviceName);

		public IServices RegisterInstance(Type serviceType, object instance) => _services.RegisterInstance(serviceType, instance);

		public IServices RegisterInstance(Type serviceType, object instance, string serviceName) => _services.RegisterInstance(serviceType, instance, serviceName);

		public IServices Register(Type serviceType) => _services.Register(serviceType);

		public IServices Register<TService>(Func<IServiceFactory, TService> factory) => _services.Register(factory);

		public IServices Register<T, TService>(Func<IServiceFactory, T, TService> factory) => _services.Register(factory);

		public IServices Register<T, TService>(Func<IServiceFactory, T, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServices Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory) => _services.Register(factory);

		public IServices Register<T1, T2, TService>(Func<IServiceFactory, T1, T2, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServices Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory) => _services.Register(factory);

		public IServices Register<T1, T2, T3, TService>(Func<IServiceFactory, T1, T2, T3, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServices Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory) => _services.Register(factory);

		public IServices Register<T1, T2, T3, T4, TService>(Func<IServiceFactory, T1, T2, T3, T4, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServices Register<TService>(Func<IServiceFactory, TService> factory, string serviceName) => _services.Register(factory, serviceName);

		public IServices RegisterFallback(Func<Type, bool> predicate, Func<Type, object> factory) => _services.RegisterFallback(predicate, factory);

		public IServices RegisterConstructorDependency<TDependency>(Func<IServiceFactory, ParameterInfo, TDependency> factory) => _services.RegisterConstructorDependency(factory);

		public IServices RegisterConstructorDependency<TDependency>(Func<IServiceFactory, ParameterInfo, object[], TDependency> factory) => _services.RegisterConstructorDependency(factory);

		public IServices Decorate(Type serviceType, Type decoratorType) => _services.Decorate(serviceType, decoratorType);

		public IServices Decorate<TService, TDecorator>() where TDecorator : TService => _services.Decorate<TService, TDecorator>();

		public IServices Decorate<TService>(Func<IServiceFactory, TService, TService> factory) => _services.Decorate(factory);
	}
}