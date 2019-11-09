using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class EnumerableCoercer<T> : IParameterizedSource<ImmutableArray<T>, IEnumerable<T>>
	{
		public static EnumerableCoercer<T> Default { get; } = new EnumerableCoercer<T>();

		EnumerableCoercer() {}

		public IEnumerable<T> Get(ImmutableArray<T> parameter) => parameter.ToArray();
	}
}