using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeDefaults : ReferenceCacheBase<TypeInfo, object>, ITypeDefaults
	{
		public static TypeDefaults Default { get; } = new TypeDefaults();

		TypeDefaults() : this(DefaultActivators.Default) {}

		readonly IActivators _activators;

		public TypeDefaults(IActivators activators)
		{
			_activators = activators;
		}

		protected override object Create(TypeInfo parameter) => _activators.Get(parameter.AsType())
		                                                                   .Get();
	}
}