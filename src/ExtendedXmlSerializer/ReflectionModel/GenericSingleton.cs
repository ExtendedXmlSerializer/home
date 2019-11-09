using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.ExtensionModel.Types;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class GenericSingleton : IGenericActivation
	{
		public static GenericSingleton Default { get; } = new GenericSingleton();

		GenericSingleton() : this(SingletonLocator.Default) {}

		readonly ISingletonLocator _singletons;

		public GenericSingleton(ISingletonLocator singletons) => _singletons = singletons;

		public Expression Get(TypeInfo parameter)
			=> Expression.Constant(_singletons.Get(parameter), parameter.AsType());
	}
}