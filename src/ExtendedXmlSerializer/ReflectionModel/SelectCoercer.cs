using System;
using System.Collections.Immutable;
using System.Linq;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class SelectCoercer<TFrom, TTo> : IParameterizedSource<ImmutableArray<TFrom>, ImmutableArray<TTo>>
	{
		readonly Func<TFrom, TTo> _select;

		public SelectCoercer(Func<TFrom, TTo> select) => _select = select;

		public ImmutableArray<TTo> Get(ImmutableArray<TFrom> parameter) => parameter.Select(_select)
		                                                                            .ToImmutableArray();
	}
}