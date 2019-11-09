using ExtendedXmlSerializer.Core.Specifications;
using System;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberAccess : DecoratedSpecification<object>, IMemberAccess
	{
		public ISpecification<object> Instance { get; }
		readonly Func<object, object>   _get;
		readonly Action<object, object> _set;

		public MemberAccess(ISpecification<object> emit, Func<object, object> get, Action<object, object> set)
			: this(emit, emit.GetInstance(), get, set) {}

		// ReSharper disable once TooManyDependencies
		public MemberAccess(ISpecification<object> emit, ISpecification<object> instance, Func<object, object> get,
		                    Action<object, object> set) : base(emit)
		{
			Instance = instance;
			_get     = get;
			_set     = set;
		}

		public object Get(object instance) => instance != null ? _get(instance) : null;

		public void Assign(object instance, object value)
		{
			if (IsSatisfiedBy(value))
			{
				_set(instance, value);
			}
		}
	}
}