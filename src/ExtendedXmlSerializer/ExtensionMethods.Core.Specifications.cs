using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISpecification&lt;TParameter&gt;.</returns>
		public static ISpecification<TParameter> IfAssigned<TParameter, TResult>(
			this IParameterizedSource<TParameter, TResult> @this)
			=> @this.ToSelectionDelegate()
			        .IfAssigned();

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISpecification&lt;TParameter&gt;.</returns>
		public static ISpecification<TParameter> IfAssigned<TParameter, TResult>(this Func<TParameter, TResult> @this)
			=> new DelegatedAssignedSpecification<TParameter, TResult>(@this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>Func&lt;T, System.Boolean&gt;.</returns>
		public static Func<T, bool> ToDelegate<T>(this ISpecification<T> @this) => @this.IsSatisfiedBy;

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>Func&lt;System.Boolean&gt;.</returns>
		public static Func<bool> Build<T>(this ISpecification<TypeInfo> @this) => @this.ToDelegate()
		                                                                               .Build(Support<T>.Key);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>Func&lt;System.Boolean&gt;.</returns>
		public static Func<bool> Build<T>(this ISpecification<T> @this, T parameter) => @this.ToDelegate()
		                                                                                     .Build(parameter);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameter">The parameter.</param>
		/// <returns>Func&lt;System.Boolean&gt;.</returns>
		public static Func<bool> Fix<T>(this ISpecification<T> @this, T parameter) => @this.ToDelegate()
		                                                                                   .FixedSelection(parameter)
		                                                                                   .ToSourceDelegate();

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>ISpecification&lt;T&gt;.</returns>
		public static ISpecification<T> Any<T>(this ISpecification<T> @this, params T[] parameters)
			=> new AnySpecification<T>();

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="others">The others.</param>
		/// <returns>ISpecification&lt;T&gt;.</returns>
		public static ISpecification<T> Or<T>(this ISpecification<T> @this, params ISpecification<T>[] others)
			=> new AnySpecification<T>(@this.Yield()
			                                .Concat(others)
			                                .Fixed());

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <param name="others">The others.</param>
		/// <returns>ISpecification&lt;T&gt;.</returns>
		public static ISpecification<T> And<T>(this ISpecification<T> @this, params ISpecification<T>[] others)
			=> new AllSpecification<T>(@this.Yield()
			                                .Concat(others)
			                                .Fixed());

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISpecification&lt;T&gt;.</returns>
		public static ISpecification<T> Inverse<T>(this ISpecification<T> @this) => new InverseSpecification<T>(@this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISpecification&lt;System.Object&gt;.</returns>
		public static ISpecification<object> AdaptForNull<T>(this ISpecification<T> @this)
			=> new NullAwareSpecificationAdapter<T>(@this);

		/// <summary>This is considered unsupported internal framework code and is not intended for external use.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="this">The this.</param>
		/// <returns>ISpecification&lt;System.Object&gt;.</returns>
		public static ISpecification<object> Adapt<T>(this ISpecification<T> @this)
			=> new SpecificationAdapter<T>(@this);
	}
}
