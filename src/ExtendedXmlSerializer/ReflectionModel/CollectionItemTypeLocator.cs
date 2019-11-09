using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class CollectionItemTypeLocator : ICollectionItemTypeLocator
	{
		public static CollectionItemTypeLocator Default { get; } = new CollectionItemTypeLocator();

		CollectionItemTypeLocator() {}

		readonly static TypeInfo ArrayInfo = typeof(Array).GetTypeInfo();
		readonly static Type     Type      = typeof(IEnumerable<>);

		// Attribution: http://stackoverflow.com/a/17713382/3602057
		public TypeInfo Get(TypeInfo parameter)
		{
			// Type is Array
			// short-circuit if you expect lots of arrays
			if (ArrayInfo.IsAssignableFrom(parameter))
				return parameter.GetElementType()
				                .GetTypeInfo();

			// type is IEnumerable<T>;
			if (parameter.IsGenericType && parameter.GetGenericTypeDefinition() == Type)
				return parameter.GetGenericArguments()[0]
				                .GetTypeInfo();

			// type implements/extends IEnumerable<T>;
			var info = parameter.GetInterfaces()
			                    .Where(t => t.GetTypeInfo()
			                                 .IsGenericType && t.GetGenericTypeDefinition() == Type)
			                    .Select(t => t.GenericTypeArguments[0])
			                    .FirstOrDefault()
			                    ?.GetTypeInfo();

			var result = (info == null) & (parameter.BaseType != null) ? Get(parameter.BaseType.GetTypeInfo()) : info;
			return result;
		}
	}
}