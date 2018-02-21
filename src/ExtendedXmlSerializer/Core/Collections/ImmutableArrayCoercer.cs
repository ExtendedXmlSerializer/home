using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	public sealed class ImmutableArrayCoercer<T> : IParameterizedSource<IEnumerable<T>, ImmutableArray<T>>
	{

		public static ImmutableArrayCoercer<T> Default { get; } = new ImmutableArrayCoercer<T>();
		ImmutableArrayCoercer() {}

		public ImmutableArray<T> Get(IEnumerable<T> parameter) => parameter.ToImmutableArray();
	}

	public sealed class YieldCoercer<T> : IParameterizedSource<T, IEnumerable<T>>
	{
		public static YieldCoercer<T> Default { get; } = new YieldCoercer<T>();
		YieldCoercer() {}

		public IEnumerable<T> Get(T parameter) => parameter.Yield();
	}

	class OfTypeCoercer<TFrom, TTo> : IParameterizedSource<IEnumerable<TFrom>, IEnumerable<TTo>> where TTo : TFrom
	{
		public static OfTypeCoercer<TFrom, TTo> Default { get; } = new OfTypeCoercer<TFrom, TTo>();
		OfTypeCoercer() {}

		public IEnumerable<TTo> Get(IEnumerable<TFrom> parameter) => parameter.OfType<TTo>();
	}

	public interface IEnumerableAlteration<T> : IAlteration<IEnumerable<T>> {}

	class OrderByAlteration<T, TMember> : IEnumerableAlteration<T>
	{
		readonly Func<T, TMember> _select;
		readonly IComparer<TMember> _comparer;

		public OrderByAlteration(Func<T, TMember> select) : this(@select, SortComparer<TMember>.Default) {}

		public OrderByAlteration(Func<T, TMember> select, IComparer<TMember> comparer)
		{
			_select = @select;
			_comparer = comparer;
		}

		public IEnumerable<T> Get(IEnumerable<T> parameter) => parameter is IOrderedEnumerable<T> ordered
			                                                       ? ordered.ThenBy(_select, _comparer)
			                                                       : parameter.OrderBy(_select, _comparer);
	}

	public class EnumerableAlterations<T> : IEnumerableAlteration<T>
	{
		readonly ImmutableArray<IAlteration<IEnumerable<T>>> _alterations;

		public EnumerableAlterations(params IAlteration<IEnumerable<T>>[] alterations) : this(alterations.ToImmutableArray()) {}

		public EnumerableAlterations(ImmutableArray<IAlteration<IEnumerable<T>>> alterations) => _alterations = alterations;

		public IEnumerable<T> Get(IEnumerable<T> parameter) => _alterations.Aggregate(parameter, (enumerable, alteration) => alteration.Get(enumerable));
	}


}
