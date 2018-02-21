using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	static class Extensions
	{
		public static THost Set<THost, T, TType>(this THost @this, IProperty<IService<T>> property, A<TType> a)
			where THost : class, IConfigurationElement
			where TType : T => @this.Set(property, a.Get());

		public static THost Set<THost, T>(this THost @this, IProperty<IService<T>> property, Type type)
			where THost : class, IConfigurationElement
			=> @this.Set(property, new Service<T>(type));

		public static THost Set<THost, T>(this THost @this, IProperty<IService<T>> property, T value)
			where THost : class, IConfigurationElement
			=> @this.Set(property, new InstanceService<T>(value));

		public static TSource Set<TSource, T, TType>(this TSource @this, IMetadataProperty<IService<T>> property, A<TType> a)
			where TSource : class, IMetadataConfiguration
			where TType : T => @this.Set(property, a.Get());

		public static TSource Set<TSource, T>(this TSource @this, IMetadataProperty<IService<T>> property, Type type)
			where TSource : class, IMetadataConfiguration => @this.Set(property, new Service<T>(type));

		public static TSource Set<TSource, T>(this TSource @this, IMetadataProperty<IService<T>> property,
		                                      T value)
			where TSource : class, IMetadataConfiguration => @this.Set(property, new InstanceService<T>(value));
	}
}