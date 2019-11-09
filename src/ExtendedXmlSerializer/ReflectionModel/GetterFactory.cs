using System;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GetterFactory : ReferenceCacheBase<MemberInfo, Func<object, object>>, IGetterFactory
	{
		public static GetterFactory Default { get; } = new GetterFactory();

		GetterFactory() {}

		static Func<object, object> Get(Type type, string name)
		{
			// Object (type object) from witch the data are retrieved
			var itemObject = Expression.Parameter(typeof(object), "item");

			// Object casted to specific type using the operator "as".
			var itemCasted = Expression.Convert(itemObject, type);

			// Property from casted object
			var property = itemCasted.PropertyOrField(type, name);

			// Because we use this function also for value type we need to add conversion to object
			var conversion = Expression.Convert(property, typeof(object));
			var lambda     = Expression.Lambda<Func<object, object>>(conversion, itemObject);
			var result     = lambda.Compile();
			return result;
		}

		protected override Func<object, object> Create(MemberInfo parameter)
			=> Get(parameter.DeclaringType, parameter.Name);
	}
}