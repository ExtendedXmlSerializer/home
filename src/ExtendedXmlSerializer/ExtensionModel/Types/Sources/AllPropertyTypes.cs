using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Types.Sources
{
	/// <summary>
	/// Iterates through all public properties in given type and resolve its types. Then recursively iterates through them.
	/// </summary>
	/// <typeparam name="T">The type to query.</typeparam>
	public sealed class AllPropertyTypes<T> : Items<Type>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public AllPropertyTypes() : base(new AllPropertyTypes(typeof(T)))
		{
			
		}
	}

	/// <summary>
	/// Iterates through all public properties in given type and resolve its types. Then recursively iterates through them.
	/// </summary>
	public sealed class AllPropertyTypes : Items<Type>
	{
		readonly static HashSet<Type> IgnoredTypes = new HashSet<Type>
		{
			typeof(string),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(short),
			typeof(ushort),
			typeof(double),
			typeof(bool),
			typeof(decimal),
			typeof(byte),
			typeof(sbyte),
			typeof(DateTime)
		};

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="type">The reference type to query.</param>
		public AllPropertyTypes(Type type) : base(GetPropertyTypes(type))
		{
			
		}

		static IEnumerable<Type> GetPropertyTypes(Type type)
		{
			var result = new HashSet<Type> { type };

			var properties = type.GetProperties()
				.Where(p => !IgnoredTypes.Contains(p.PropertyType))
				.Where(p => p.GetCustomAttribute<XmlIgnoreAttribute>() == null);

			foreach (var property in properties)
			{
				var propertyType = property.PropertyType;
				if (IsSupportedGenericType(propertyType))
				{
					propertyType = propertyType.GenericTypeArguments.First();
				}

				if (!result.Contains(propertyType))
				{
					result.UnionWith(GetPropertyTypes(propertyType));
				}
			}

			return result;
		}

		static bool IsSupportedGenericType(Type propertyType)
		{
			return typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType.IsGenericType && propertyType.GenericTypeArguments.Length == 1;
		}
	}


}