using ExtendedXmlSerializer.Core.Sources;
using System;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class DefaultConstructedActivators : DecoratedSource<Type, IActivator>, IActivators
	{
		public static DefaultConstructedActivators Default { get; } = new DefaultConstructedActivators();

		DefaultConstructedActivators() : base(new ConstructedActivators(ConstructorLocator.Default)) {}
	}
}