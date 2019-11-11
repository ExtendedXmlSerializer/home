using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtendedXmlSerializer
{
	// ReSharper disable TooManyArguments
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>IAlteration&lt;System.Object&gt;.</returns>
		public static IAlteration<object> Adapt<T>(this IAlteration<T> @this) => new AlterationAdapter<T>(@this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>T.</returns>
		public static T Get<T>(this IParameterizedSource<Stream, T> @this, string parameter)
			=> @this.Get(new MemoryStream(Encoding.UTF8.GetBytes(parameter)));

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>T.</returns>
		public static T Get<T>(this IParameterizedSource<TypeInfo, T> @this, Type parameter)
			=> @this.Get(parameter.GetTypeInfo());

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>T.</returns>
		public static T Get<T>(this IParameterizedSource<Type, T> @this, TypeInfo parameter)
			=> @this.Get(parameter.AsType());


		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <typeparam name="TTo">The type of the t to.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="coercer">The coercer.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TTo&gt;.</returns>
		public static IParameterizedSource<TParameter, TTo> To<TParameter, TResult, TTo>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TResult, TTo> coercer)
			=> new CoercedResult<TParameter, TResult, TTo>(@this, coercer);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TFrom">The type of the t from.</typeparam>
		/// <typeparam name="TTo">The type of the t to.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="coercer">The coercer.</param>
		/// <returns>IParameterizedSource&lt;TFrom, TResult&gt;.</returns>
		public static IParameterizedSource<TFrom, TResult> In<TFrom, TTo, TResult>(
			this IParameterizedSource<TTo, TResult> @this, IParameterizedSource<TFrom, TTo> coercer)
			=> new CoercedParameter<TFrom, TTo, TResult>(coercer, @this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TItem">The type of the t item.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>System.Nullable&lt;ImmutableArray&lt;TItem&gt;&gt;.</returns>
		public static ImmutableArray<TItem>? GetAny<T, TItem>(this IParameterizedSource<T, ImmutableArray<TItem>> @this,
		                                                      T parameter)
		{
			var items  = @this.Get(parameter);
			var result = items.Any() ? items : (ImmutableArray<TItem>?)null;
			return result;
		}

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="specification">The specification.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TResult&gt;.</returns>
		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification)
			=> new ConditionalSource<TParameter, TResult>(specification, @this,
			                                              new FixedInstanceSource<TParameter, TResult>(default));

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="specification">The specification.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TResult&gt;.</returns>
		public static IParameterizedSource<TParameter, TResult> If<TParameter, TResult>(
			this TResult @this, ISpecification<TParameter> specification)
			=> new ConditionalSource<TParameter, TResult>(specification,
			                                              new FixedInstanceSource<TParameter, TResult>(@this),
			                                              new FixedInstanceSource<TParameter, TResult>(default));

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="specification">The specification.</param>
		/// <param name="other">The other.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TResult&gt;.</returns>
		public static IParameterizedSource<TParameter, TResult> Let<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			IParameterizedSource<TParameter, TResult> other) =>
			Let(@this, specification, AlwaysSpecification<TResult>.Default, other);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="specification">The specification.</param>
		/// <param name="result">The result.</param>
		/// <param name="other">The other.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TResult&gt;.</returns>
		public static IParameterizedSource<TParameter, TResult> Let<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			ISpecification<TResult> result,
			IParameterizedSource<TParameter, TResult> other)
			=> new ConditionalSource<TParameter, TResult>(specification, result, other, @this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="specification">The specification.</param>
		/// <param name="other">The other.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TResult&gt;.</returns>
		public static IParameterizedSource<TParameter, TResult> Let<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, ISpecification<TParameter> specification,
			TResult other)
			=> new ConditionalSource<TParameter, TResult>(specification,
			                                              new FixedInstanceSource<TParameter, TResult>(other),
			                                              @this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TSpecification">The type of the t specification.</typeparam>
		/// <typeparam name="TInstance">The type of the t instance.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="specification">The specification.</param>
		/// <param name="other">The other.</param>
		/// <returns>IParameterizedSource&lt;TSpecification, TInstance&gt;.</returns>
		public static IParameterizedSource<TSpecification, TInstance> Let<TSpecification, TInstance>(
			this TInstance @this, ISpecification<TSpecification> specification,
			TInstance other)
			=> new ConditionalSource<TSpecification, TInstance>(specification,
			                                                    new FixedInstanceSource<TSpecification, TInstance
			                                                    >(other),
			                                                    new FixedInstanceSource<TSpecification, TInstance
			                                                    >(@this));

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="next">The next.</param>
		/// <returns>IParameterizedSource&lt;TParameter, TResult&gt;.</returns>
		public static IParameterizedSource<TParameter, TResult> Or<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this, IParameterizedSource<TParameter, TResult> next)
			where TResult : class => new LinkedDecoratedSource<TParameter, TResult>(@this, next);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="seed">The seed.</param>
		/// <returns>T.</returns>
		public static T Alter<T>(this IEnumerable<IAlteration<T>> @this, T seed)
			=> @this.Aggregate(seed, (current, alteration) => alteration.Get(current));

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>Func&lt;TResult&gt;.</returns>
		public static Func<TResult> Build<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.ToSelectionDelegate()
			        .Build(parameter);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>Func&lt;TResult&gt;.</returns>
		public static Func<TResult> Build<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                       TParameter parameter)
			=> @this.FixedSelection(parameter)
			        .Singleton()
			        .Get;

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>ISource&lt;TResult&gt;.</returns>
		public static ISource<TResult> FixedSelection<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this,
			TParameter parameter) => @this.ToSelectionDelegate()
			                              .FixedSelection(parameter);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>ISource&lt;TResult&gt;.</returns>
		public static ISource<TResult> FixedSelection<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                                   TParameter parameter)
			=> new FixedSource<TParameter, TResult>(@this, parameter);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISource&lt;T&gt;.</returns>
		public static ISource<T> Singleton<T>(this ISource<T> @this) => new SingletonSource<T>(@this.Get);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <returns>Func&lt;TParameter, TResult&gt;.</returns>
		public static Func<TParameter, TResult> ToSelectionDelegate<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this) => @this.Get;

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>Func&lt;T&gt;.</returns>
		public static Func<T> ToSourceDelegate<T>(this ISource<T> @this) => @this.Get;

		#region Obsolete

		[Obsolete("Use ToSourceDelegate instead.")]
		public static Func<T> ToDelegate<T>(this ISource<T> @this) => @this.ToSourceDelegate();

		[Obsolete("Use ToSelectionDelegate instead.")]
		public static Func<TParameter, TResult> ToDelegate<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this) => @this.ToSelectionDelegate();

		[Obsolete("Use FixedSelection instead.")]
		public static ISource<TResult> Fix<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> @this.FixedSelection(parameter);

		[Obsolete("Use FixedSelection instead.")]
		public static ISource<TResult> Fix<TParameter, TResult>(this Func<TParameter, TResult> @this,
		                                                        TParameter parameter)
			=> @this.FixedSelection(parameter);

		#endregion
	}
}