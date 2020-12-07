using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ConstructedActivators : ReferenceCacheBase<Type, IActivator>, IActivators
	{
		readonly static Func<ParameterInfo, Expression> Selector = DefaultParameters.Instance.Get;

		/*public static DefaultActivators Default { get; } = new DefaultActivators();

		DefaultActivators() : this(ConstructorLocator.Default) {}*/

		readonly IConstructorLocator _locator;

		public ConstructedActivators(IConstructorLocator locator) => _locator = locator;

		protected override IActivator Create(Type parameter)
		{
			var expression = parameter == typeof(string)
				                 ? (Expression)Expression.Default(parameter)
				                 : parameter.IsValueType
					                 ? Expression.New(parameter)
					                 : Reference(parameter);
			var convert = Expression.Convert(expression, typeof(object));
			var lambda  = Expression.Lambda<Func<object>>(convert);
			var result  = new Activator(lambda.Compile());
			return result;
		}

		NewExpression Reference(Type parameter)
		{
			var constructor = _locator.Get(parameter);
			var parameters  = constructor.GetParameters();
			var result = parameters.Length > 0
				             ? Expression.New(constructor, parameters.Select(Selector))
				             : Expression.New(constructor);
			return result;
		}

		sealed class DefaultParameters : IParameterizedSource<ParameterInfo, Expression>
		{
			readonly static IEnumerable<Expression> Initializers = Enumerable.Empty<Expression>();

			public static DefaultParameters Instance { get; } = new DefaultParameters();

			DefaultParameters() {}

			public Expression Get(ParameterInfo parameter)
				=> parameter.IsDefined(typeof(ParamArrayAttribute))
					   ? (Expression)Expression.NewArrayInit(parameter.ParameterType.GetElementType() ??
					                                         throw new
						                                         InvalidOperationException("Element Type not found."),
					                                         Initializers)
					   : Expression.Default(parameter.ParameterType);
		}
	}
}