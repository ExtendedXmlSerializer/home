using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Collections
{
	public sealed class ImmutableArrayCoercer<T> : IParameterizedSource<IEnumerable<T>, ImmutableArray<T>>
	{

		public static ImmutableArrayCoercer<T> Default { get; } = new ImmutableArrayCoercer<T>();
		ImmutableArrayCoercer() {}

		public ImmutableArray<T> Get(IEnumerable<T> parameter) => parameter.ToImmutableArray();
	}
}
