using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	class DelegatedSource<T> : ISource<T>
	{
		readonly Func<T> _source;

		public DelegatedSource(Func<T> source) => _source = source;

		public T Get() => _source();
	}

	class DelegatedSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly Func<TParameter, TResult> _source;

		public DelegatedSource(Func<TParameter, TResult> source) => _source = source;

		public virtual TResult Get(TParameter parameter) => _source(parameter);
	}
}