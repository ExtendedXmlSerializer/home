using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	static class Extensions
	{
		public static IMetadataConfiguration Set<TValue, T>(this IMetadataConfiguration @this,
		                                                    IProperty<IService<TValue>> property,
		                                                    A<T> a) where T : TValue => @this.Set(property, a.Get());

		public static IMetadataConfiguration Set<T>(this IMetadataConfiguration @this, IProperty<IService<T>> property,
		                                            Type type) => @this.Set(property, new Service<T>(type));

		public static IMetadataConfiguration Set<T>(this IMetadataConfiguration @this, IProperty<IService<T>> property,
		                                            T value) => @this.Set(property, new InstanceService<T>(value));
	}
}