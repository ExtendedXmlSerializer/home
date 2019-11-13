using System;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public class Property<TType, TValue> : IProperty
	{
		readonly static ISpecification<TypeInfo> Accepts  = IsAssignableSpecification<TType>.Default;
		readonly static TypeInfo                 TypeInfo = Support<TValue>.Metadata;

		readonly ISpecification<TypeInfo>    _specification;
		readonly ITableSource<TType, TValue> _store;

		public Property(ITableSource<TType, TValue> store, Expression<Func<IProperty>> source) :
			this(Accepts, store, source) {}

		public Property(ISpecification<TypeInfo> specification, ITableSource<TType, TValue> store,
		                Expression<Func<IProperty>> source)

		{
			_specification = specification;
			_store         = store;
			Metadata = source.GetMemberInfo()
			                 .AsValid<PropertyInfo>();
		}

		public PropertyInfo Metadata { get; }

		bool ISpecification<object>.IsSatisfiedBy(object parameter)
			=> parameter is TType && IsSatisfiedBy((TType)parameter);

		object IParameterizedSource<object, object>.Get(object parameter) => Get(parameter.AsValid<TType>());

		void IAssignable<object, object>.Assign(object key, object value)
			=> Assign(key.AsValid<TType>(), value.AsValid<TValue>());

		public bool IsSatisfiedBy(TType parameter) => _store.IsSatisfiedBy(parameter);

		public TValue Get(TType parameter) => _store.Get(parameter);

		public void Assign(TType key, TValue value) => _store.Assign(key, value);

		public TypeInfo Get() => TypeInfo;

		public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

		bool ITableSource<object, object>.Remove(object key) => Remove(key.AsValid<TType>());

		public bool Remove(TType key) => _store.Remove(key);
	}
}