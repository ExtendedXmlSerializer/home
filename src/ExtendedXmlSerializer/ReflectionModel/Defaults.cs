using System;
using System.Linq.Expressions;

namespace ExtendedXmlSerializer.ReflectionModel
{
	static class Defaults
	{
		public static ITypeComparer TypeComparer { get; } = new CompositeTypeComparer(ImplementedTypeComparer.Default,
		                                                                              TypeIdentityComparer.Default);

		public static Func<Type, ParameterExpression> Parameter { get; } = Expression.Parameter;

		public static Func<ParameterExpression, Type, Expression> ExpressionZip { get; }
			= (expression, type) => type.IsAssignableFrom(expression.Type)
				                        ? expression
				                        : (Expression)Expression.Convert(expression, type);
	}
}