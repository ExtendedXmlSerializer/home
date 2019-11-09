using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class PublicConstructorLocator : IConstructorLocator
	{
		public static PublicConstructorLocator Default { get; } = new PublicConstructorLocator();

		PublicConstructorLocator() {}

		public ConstructorInfo Get(TypeInfo parameter) => parameter.GetConstructors()
		                                                           .FirstOrDefault();
	}
}