using System;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AddDelegates : ReferenceCacheBase<TypeInfo, Action<object, object>>, IAddDelegates
	{
		public static AddDelegates Default { get; } = new AddDelegates();

		AddDelegates() : this(CollectionItemTypeLocator.Default, AddMethodLocator.Default) {}

		readonly ICollectionItemTypeLocator _locator;
		readonly IAddMethodLocator          _add;

		public AddDelegates(ICollectionItemTypeLocator locator, IAddMethodLocator add)
		{
			_locator = locator;
			_add     = add;
		}

		protected override Action<object, object> Create(TypeInfo parameter)
		{
			var elementType = _locator.Get(parameter);
			if (elementType != null)
			{
				var add = _add.Locate(parameter, elementType);
				if (add != null)
				{
					// Object (type object) from witch the data are retrieved
					var itemObject = Expression.Parameter(typeof(object), "item");
					var value      = Expression.Parameter(typeof(object), "value");

					// Object casted to specific type using the operator "as".
					var itemCasted = Expression.Convert(itemObject, parameter.AsType());

					var castedParam = Expression.Convert(value, elementType.AsType());

					var conversion = Expression.Call(itemCasted, add, castedParam);

					var lambda = Expression.Lambda<Action<object, object>>(conversion, itemObject, value);

					var result = lambda.Compile();
					return result;
				}
			}

			return null;
		}
	}
}