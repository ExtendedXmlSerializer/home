using System;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using LightInject;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ConstructorSelector : IConstructorSelector
	{
		public static ConstructorSelector Default { get; } = new ConstructorSelector();

		ConstructorSelector() : this(Constructors.Default) {}

		readonly IConstructors _constructors;

		public ConstructorSelector(IConstructors constructors) => _constructors = constructors;

		public ConstructorInfo Execute(Type implementingType) => _constructors.Get(implementingType.GetTypeInfo())
		                                                                      .OrderBy(c => c.GetParameters()
		                                                                                     .Length)
		                                                                      .First();
	}
}