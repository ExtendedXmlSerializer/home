using ExtendedXmlSerializer.ExtensionModel.Types;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	static class Support<T>
	{
		public static Type Key { get; } = typeof(T);

		public static TypeInfo Metadata { get; } = typeof(T).GetTypeInfo();

		public static Func<T> New { get; } = DefaultActivators.Default.New<T>;

		public static Func<T> NewOrSingleton { get; } = Activators.Default.New<T>;
	}
}