using System;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	abstract class GenericAdapterBase<T> : DecoratedSource<ImmutableArray<TypeInfo>, T>
	{
		protected GenericAdapterBase(Type definition, IParameterizedSource<TypeInfo, T> source)
			: base(
			       new SelectCoercer<TypeInfo, Type>(TypeCoercer.Default.ToDelegate())
				       .To(new GenericTypeAlteration(definition))
				       .To(TypeMetadataCoercer.Default)
				       .To(source)
			      ) {}
	}
}