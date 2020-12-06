using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class DefaultValueTypeDefaults : ITypeDefaults
	{
		public static DefaultValueTypeDefaults Default { get; } = new DefaultValueTypeDefaults();

		DefaultValueTypeDefaults() : this(new Generic<ISource<object>>(typeof(DefaultValues<>))) {}

		readonly IGeneric<ISource<object>> _generic;

		public DefaultValueTypeDefaults(IGeneric<ISource<object>> generic) => _generic = generic;

		public object Get(TypeInfo parameter) => _generic.Get(parameter)().Get();
	}
}