using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	static class Extensions
	{
		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, ISource<TKey> key, TValue instance)
			=> @this.Assign(key.Get(), instance);

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, ISource<TKey> key, Type type)
			=> @this.Assign(key.Get(), type);

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue, T>(this IAssignable<TKey, IService<TValue>> @this, ISource<TKey> key, A<T> a) where T : TValue
			=> @this.Assign(key.Get(), a);

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, TKey key, TValue instance)
			=> @this.Assign(key, new InstanceService<TValue>(instance));

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, TKey key, Type type)
			=> @this.Assign(key, new Service<TValue>(type));

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue, T>(this IAssignable<TKey, IService<TValue>> @this, TKey key, A<T> a) where T : TValue
			=> @this.Assign(key, new Service<TValue>(a.Get()));

		/*public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, TKey key, TValue instance)
			=> @this.Assign(key, new InstanceService<TValue>(instance));

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue>(this IAssignable<TKey, IService<TValue>> @this, TKey key, Type type)
			=> @this.Assign(key, new Service<TValue>(type));

		public static IAssignable<TKey, IService<TValue>> Assign<TKey, TValue, T>(this IAssignable<TKey, IService<TValue>> @this, TKey key, A<T> a) where T : TValue
			=> @this.Assign(key, new Service<TValue>(a.Get()));*/
	}


}
