namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class AlteredSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly IAlteration<TResult>                      _alteration;
		readonly IParameterizedSource<TParameter, TResult> _source;

		public AlteredSource(IAlteration<TResult> alteration, IParameterizedSource<TParameter, TResult> source)
		{
			_alteration = alteration;
			_source     = source;
		}

		public TResult Get(TParameter parameter) => _alteration.Get(_source.Get(parameter));
	}
}