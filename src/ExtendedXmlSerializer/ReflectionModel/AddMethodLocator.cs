using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AddMethodLocator : IAddMethodLocator
	{
		public static AddMethodLocator Default { get; } = new AddMethodLocator();

		AddMethodLocator() {}

		const string Add = "Add";

		public MethodInfo Locate(TypeInfo type, TypeInfo elementType) => Get(type, elementType);

		static MethodInfo Get(TypeInfo type, TypeInfo elementType)
		{
			foreach (var candidate in AllInterfaces.Default.Get(type))
			{
				var method     = candidate.GetMethod(Add);
				var parameters = method?.GetParameters();
				if (parameters?.Length == 1 && elementType.IsAssignableFrom(parameters[0]
					                                                            .ParameterType))
				{
					return method;
				}
			}

			return null;
		}
	}
}