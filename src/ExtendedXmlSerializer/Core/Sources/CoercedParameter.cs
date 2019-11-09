namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class CoercedParameter<TFrom, TTo, TResult> : IParameterizedSource<TFrom, TResult>
	{
		readonly IParameterizedSource<TFrom, TTo>   _coercer;
		readonly IParameterizedSource<TTo, TResult> _source;

		public CoercedParameter(IParameterizedSource<TFrom, TTo> coercer, IParameterizedSource<TTo, TResult> source)
		{
			_coercer = coercer;
			_source  = source;
		}

		public TResult Get(TFrom parameter) => _source.Get(_coercer.Get(parameter));
	}
}