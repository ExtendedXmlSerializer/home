namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class CoercedResult<TParameter, TResult, TTo> : IParameterizedSource<TParameter, TTo>
	{
		readonly IParameterizedSource<TResult, TTo>        _coercer;
		readonly IParameterizedSource<TParameter, TResult> _source;

		public CoercedResult(IParameterizedSource<TParameter, TResult> source,
		                     IParameterizedSource<TResult, TTo> coercer)
		{
			_coercer = coercer;
			_source  = source;
		}

		public TTo Get(TParameter parameter) => _coercer.Get(_source.Get(parameter));
	}
}