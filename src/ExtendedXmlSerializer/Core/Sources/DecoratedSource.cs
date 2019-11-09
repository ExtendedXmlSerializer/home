namespace ExtendedXmlSerializer.Core.Sources
{
	class DecoratedSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly IParameterizedSource<TParameter, TResult> _source;

		public DecoratedSource(IParameterizedSource<TParameter, TResult> source) => _source = source;

		public TResult Get(TParameter parameter) => _source.Get(parameter);
	}
}