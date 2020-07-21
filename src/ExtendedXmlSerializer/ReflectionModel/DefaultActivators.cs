using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DefaultActivators : ReferenceCacheBase<Type, IActivator>, IActivators
	{
		readonly static Func<ParameterInfo, Expression> Selector = DefaultParameters.Instance.Get;

		public static DefaultActivators Default { get; } = new DefaultActivators();

		DefaultActivators() : this(ConstructorLocator.Default, IsCollectionType.Instance) {}

		readonly IConstructorLocator      _locator;
		readonly ISpecification<TypeInfo> _collection;

		public DefaultActivators(IConstructorLocator locator, ISpecification<TypeInfo> collection)
		{
			_locator    = locator;
			_collection = collection;
		}

		protected override IActivator Create(Type parameter)
		{
			var typeInfo = parameter.GetTypeInfo();
			var expression = parameter == typeof(string)
				                 ? (Expression)Expression.Default(parameter)
				                 : typeInfo.IsValueType
					                 ? Expression.New(parameter)
					                 : Reference(parameter, typeInfo);
			var convert = Expression.Convert(expression, typeof(object));
			var lambda  = Expression.Lambda<Func<object>>(convert);
			var result  = new Activator(lambda.Compile());
			return result;
		}

		NewExpression Reference(Type parameter, TypeInfo typeInfo)
		{
			var accounted = typeInfo.IsInterface && _collection.IsSatisfiedBy(parameter)
				                ? typeof(List<>).MakeGenericType(CollectionItemTypeLocator.Default.Get(typeInfo))
				                : typeInfo;
			var constructor = _locator.Get(accounted) ?? _locator.Get(typeInfo);
			var parameters  = constructor.GetParameters();
			var result = parameters.Length > 0
				             ? Expression.New(constructor, parameters.Select(Selector))
				             : Expression.New(accounted);
			return result;
		}

		sealed class DefaultParameters : IParameterizedSource<ParameterInfo, Expression>
		{
			readonly static IEnumerable<Expression> Initializers = Enumerable.Empty<Expression>();

			public static DefaultParameters Instance { get; } = new DefaultParameters();

			DefaultParameters() {}

			public Expression Get(ParameterInfo parameter)
				=> parameter.IsDefined(typeof(ParamArrayAttribute))
					   ? (Expression)Expression.NewArrayInit(
					                                         parameter.ParameterType.GetElementType() ??
					                                         throw new
						                                         InvalidOperationException("Element Type not found."),
					                                         Initializers)
					   : Expression.Default(parameter.ParameterType);
		}

		sealed class IsCollectionType : AnySpecification<TypeInfo>
		{
			public static IsCollectionType Instance { get; } = new IsCollectionType();

			IsCollectionType() : base(IsCollectionTypeSpecification.Default,
			                          new IsAssignableGenericSpecification(typeof(IReadOnlyCollection<>))) {}
		}
	}
}