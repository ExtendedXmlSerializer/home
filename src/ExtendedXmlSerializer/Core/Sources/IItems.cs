using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Sources
{
	interface IItems<T> : IEnumerable<T>, ISource<ImmutableArray<T>> {}
}