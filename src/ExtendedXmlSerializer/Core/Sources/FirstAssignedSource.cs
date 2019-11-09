using System.Collections.Immutable;

namespace ExtendedXmlSerializer.Core.Sources
{
	class FirstAssignedSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly TResult                                                   _defaultValue;
		readonly ImmutableArray<IParameterizedSource<TParameter, TResult>> _sources;

		public FirstAssignedSource(params IParameterizedSource<TParameter, TResult>[] sources) :
			this(default, sources) {}

		public FirstAssignedSource(TResult defaultValue, params IParameterizedSource<TParameter, TResult>[] sources)
		{
			_defaultValue = defaultValue;
			_sources      = sources.ToImmutableArray();
		}

		public TResult Get(TParameter parameter)
		{
			var length = _sources.Length;
			for (var i = 0; i < length; i++)
			{
				var result = _sources[i]
					.Get(parameter);
				if (!Equals(result, _defaultValue))
				{
					return result;
				}
			}

			return default;
		}
	}
}