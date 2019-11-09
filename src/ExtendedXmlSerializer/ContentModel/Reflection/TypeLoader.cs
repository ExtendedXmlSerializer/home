using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeLoader : ITypePartitions
	{
		public static TypeLoader Default { get; } = new TypeLoader();

		TypeLoader() : this(TypeNameAlteration.Default) {}

		readonly IAlteration<string> _names;

		public TypeLoader(IAlteration<string> names)
		{
			_names = names;
		}

		public ImmutableArray<TypeInfo>? Get(TypePartition parameter)
			=> parameter.Assembly
			            .GetType($"{parameter.Namespace}.{_names.Get(parameter.Name)}", false, false)
			            ?
			            .GetTypeInfo()
			            .Yield()
			            .ToImmutableArray();
	}
}