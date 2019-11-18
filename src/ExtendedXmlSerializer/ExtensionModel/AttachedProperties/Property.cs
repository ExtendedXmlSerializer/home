using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	/// <summary>
	/// A baseline property component that represents an attached property.
	/// </summary>
	/// <typeparam name="TType">The hosting type.</typeparam>
	/// <typeparam name="TValue">The property's value.</typeparam>
	public class Property<TType, TValue> : IProperty
	{
		readonly static ISpecification<TypeInfo> Accepts  = IsAssignableSpecification<TType>.Default;
		readonly static TypeInfo                 TypeInfo = Support<TValue>.Metadata;

		readonly ISpecification<TypeInfo>    _specification;
		readonly ITableSource<TType, TValue> _store;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="store"></param>
		/// <param name="source"></param>
		public Property(ITableSource<TType, TValue> store, Expression<Func<IProperty>> source)
			: this(Accepts, store, source) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification"></param>
		/// <param name="store"></param>
		/// <param name="source"></param>
		public Property(ISpecification<TypeInfo> specification, ITableSource<TType, TValue> store,
		                Expression<Func<IProperty>> source)

		{
			_specification = specification;
			_store         = store;
			Metadata       = source.GetMemberInfo().AsValid<PropertyInfo>();
		}

		/// <inheritdoc />
		public PropertyInfo Metadata { get; }

		bool ISpecification<object>.IsSatisfiedBy(object parameter)
			=> parameter is TType type && IsSatisfiedBy(type);

		object IParameterizedSource<object, object>.Get(object parameter) => Get(parameter.AsValid<TType>());

		void IAssignable<object, object>.Assign(object key, object value)
			=> Assign(key.AsValid<TType>(), value.AsValid<TValue>());

		/// <inheritdoc cref="ISpecification{T}" />
		public bool IsSatisfiedBy(TType parameter) => _store.IsSatisfiedBy(parameter);

		/// <inheritdoc cref="ITableSource{TKey,TValue}" />
		public TValue Get(TType parameter) => _store.Get(parameter);

		/// <inheritdoc cref="ITableSource{TKey,TValue}" />
		public void Assign(TType key, TValue value) => _store.Assign(key, value);

		/// <inheritdoc cref="ISource{T}" />
		public TypeInfo Get() => TypeInfo;

		/// <inheritdoc />
		public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

		bool ITableSource<object, object>.Remove(object key) => Remove(key.AsValid<TType>());

		/// <inheritdoc cref="ITableSource{TKey,TValue}" />
		public bool Remove(TType key) => _store.Remove(key);
	}
}