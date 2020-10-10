using ExtendedXmlSerializer.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DictionaryPairTypesLocator : IDictionaryPairTypesLocator
	{
		public static DictionaryPairTypesLocator Default { get; } = new DictionaryPairTypesLocator();

		DictionaryPairTypesLocator() : this(typeof(IDictionary<,>)) {}

		readonly Type _type;

		public DictionaryPairTypesLocator(Type type) => _type = type;

		public DictionaryPairTypes? Get(TypeInfo parameter)
		{
			foreach (var it in parameter.GetInterfaces()
			                            .ToMetadata()
			                            .Add(parameter))
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == _type)
				{
					var arguments = it.GetGenericArguments();
					var mapping   = new DictionaryPairTypes(arguments[0].GetTypeInfo(), arguments[1].GetTypeInfo());
					return mapping;
				}
			}

			var baseType = parameter.BaseType?.GetTypeInfo();
			var result   = baseType != null ? Get(baseType) : null;
			return result;
		}
	}
}