namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class LinkedDecoratedSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
		where TResult : class
	{
		readonly IParameterizedSource<TParameter, TResult> _source;
		readonly IParameterizedSource<TParameter, TResult> _next;

		public LinkedDecoratedSource(IParameterizedSource<TParameter, TResult> source,
		                             IParameterizedSource<TParameter, TResult> next)
		{
			_source = source;
			_next   = next;
		}

		public TResult Get(TParameter parameter) => _source.Get(parameter) ?? _next.Get(parameter);
	}
}