using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class DefaultTypeDefaults : DecoratedSource<TypeInfo, object>, ITypeDefaults
	{
		public static DefaultTypeDefaults Default { get; } = new DefaultTypeDefaults();

		DefaultTypeDefaults() : base(new TypeDefaults(DefaultConstructedActivators.Default)) {}
	}
}