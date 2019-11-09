using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class SetterFactory : ISetterFactory
	{
		public static SetterFactory Default { get; } = new SetterFactory();

		SetterFactory() {}

		public Action<object, object> Get(MemberInfo parameter) => Get(parameter.DeclaringType, parameter.Name);

		static Action<object, object> Get(Type type, string name)
		{
			// Object (type object) from witch the data are retrieved
			var itemObject = Expression.Parameter(typeof(object), "item");

			// Object casted to specific type using the operator "as".
			var itemCasted = type.GetTypeInfo()
			                     .IsValueType
				                 ? Expression.Unbox(itemObject, type)
				                 : Expression.Convert(itemObject, type);
			// Property from casted object
			var property = itemCasted.PropertyOrField(type, name);

			// Secound parameter - value to set
			var value = Expression.Parameter(typeof(object), "value");

			// Because we use this function also for value type we need to add conversion to object
			var paramCasted = Expression.Convert(value, property.Type);

			// Assign value to property
			var assign = Expression.Assign(property, paramCasted);

			var lambda = Expression.Lambda<Action<object, object>>(assign, itemObject, value);

			var result = lambda.Compile();
			return result;
		}
	}
}