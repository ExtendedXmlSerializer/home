using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public static class MemberExpressions
	{
		public static MemberExpression PropertyOrField(this UnaryExpression expression, Type type, string name)
		{
			var typeInfo = type.GetTypeInfo();
			var property = typeInfo.GetDeclaredProperty(name) ??
			               typeInfo.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
			if (property != null)
			{
				return Expression.Property(expression, property);
			}

			var field = type.GetField(name) ?? type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			if (field != null)
			{
				return Expression.Field(expression, field);
			}

			throw new InvalidOperationException($"Could not find the member '{name}' on type '{type}'.");
		}
	}
}