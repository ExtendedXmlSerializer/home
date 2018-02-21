using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	static class Extensions
	{
		public static THost Set<THost, T, TType>(this THost @this, IProperty<ISource<IMetadata>, IService<T>> property, A<TType> a)
			where THost : class, IConfigurationElement, ISource<IMetadata>
			where TType : T => @this.Set(property, a.Get());

		public static THost Set<THost, T>(this THost @this, IProperty<ISource<IMetadata>, IService<T>> property,
		                                  Type type) 
			where THost : class, IConfigurationElement, ISource<IMetadata>
			=> @this.Set(property, new Service<T>(type));

		public static THost Set<THost, T>(this THost @this, IProperty<ISource<IMetadata>, IService<T>> property,
		                                  T value) 
			where THost : class, IConfigurationElement, ISource<IMetadata>
			=> @this.Set(property, new InstanceService<T>(value));
	}
}