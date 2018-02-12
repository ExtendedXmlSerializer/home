using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	static class Extensions
	{
		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, TKey key, TValue instance)
			=> @this.Assign(key, new InstanceService<TValue>(instance));

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, TKey key, Type type)
			=> @this.Assign(key, new Service<TValue>(type));

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue, T>(this IAssignable<TKey, IService<TValue>> @this, TKey key, A<T> a) where T : TValue
			=> @this.Assign(key, new Service<TValue>(a.Get()));
	}


}
