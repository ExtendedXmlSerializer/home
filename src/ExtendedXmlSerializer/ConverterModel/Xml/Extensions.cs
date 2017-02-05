using System;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	public static class Extensions
	{
		public static string GetArguments(this IFormatter<TypeInfo> @this, TypeInfo type)
			=> @this.GetArguments(type.GetGenericArguments());

		public static string GetArguments(this IFormatter<TypeInfo> @this, params Type[] parameter)
			=> string.Join(",", Generic(@this, parameter));

		static IEnumerable<string> Generic(IFormatter<TypeInfo> formatter, IReadOnlyList<Type> types)
		{
			for (var i = 0; i < types.Count; i++)
			{
				yield return formatter.Get(types[i].GetTypeInfo());
			}
		}
	}
}