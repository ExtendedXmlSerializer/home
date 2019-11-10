using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	abstract class GenericAdapterBase<T> : DecoratedSource<ImmutableArray<TypeInfo>, T>
	{
		protected GenericAdapterBase(Type definition, IParameterizedSource<TypeInfo, T> source)
			: base(
			       new SelectCoercer<TypeInfo, Type>(TypeCoercer.Default.ToSelectionDelegate())
				       .To(new GenericTypeAlteration(definition))
				       .To(TypeMetadataCoercer.Default)
				       .To(source)
			      ) {}
	}
}