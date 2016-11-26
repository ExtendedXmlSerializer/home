using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerialization.Cache
{
	public interface IElementTypeLocator
	{
		Type Locate( Type type );
	}

	public class ElementTypeLocator : IElementTypeLocator
	{
		readonly static TypeInfo ArrayInfo = typeof(Array).GetTypeInfo();
		public static ElementTypeLocator Default { get; } = new ElementTypeLocator();
		ElementTypeLocator() {}

		// Attribution: http://stackoverflow.com/a/17713382/3602057
		public Type Locate( Type type )
		{
			// Type is Array
			// short-circuit if you expect lots of arrays 
			if ( ArrayInfo.IsAssignableFrom( type ) )
				return type.GetElementType();

			// type is IEnumerable<T>;
			if ( type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) )
				return type.GetGenericArguments()[0];

			// type implements/extends IEnumerable<T>;
			var result = type.GetInterfaces()
							 .Where( t => t.GetTypeInfo().IsGenericType &&
										  t.GetGenericTypeDefinition() == typeof(IEnumerable<>) )
							 .Select( t => t.GenericTypeArguments[0] ).FirstOrDefault();
			
			return result;
		}
	}
}
