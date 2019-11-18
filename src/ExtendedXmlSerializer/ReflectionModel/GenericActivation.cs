using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GenericActivation : IGenericActivation
	{
		readonly ImmutableArray<Type>                _types;
		readonly ImmutableArray<ParameterExpression> _expressions;

		public GenericActivation(ImmutableArray<Type> types, ImmutableArray<ParameterExpression> expressions)
		{
			_types       = types;
			_expressions = expressions;
		}

		public Expression Get(TypeInfo parameter)
		{
			var constructor = parameter.DeclaredConstructors.Only() ??
			                  parameter.GetConstructors()
			                           .Only() ??
			                  parameter.GetConstructor(_types.ToArray());
			var expressions = _expressions.ToArray();
			var types = constructor.GetParameters()
			                       .Select(x => x.ParameterType);
			var parameters = expressions.Zip(types, Defaults.ExpressionZip);
			var result     = Expression.New(constructor, parameters);
			return result;
		}
	}
}