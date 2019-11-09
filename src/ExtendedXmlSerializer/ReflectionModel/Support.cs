using System;
using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Types;

namespace ExtendedXmlSerializer.ReflectionModel
{
	static class Support<T>
	{
		public static TypeInfo Key { get; } = typeof(T).GetTypeInfo();

		public static Func<T> New { get; } = DefaultActivators.Default.New<T>;

		public static Func<T> NewOrSingleton { get; } = Activators.Default.New<T>;
	}
}