using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	class FixedSource<TParameter, TResult> : ISource<TResult>
	{
		readonly Func<TParameter, TResult> _source;
		readonly TParameter                _parameter;

		public FixedSource(IParameterizedSource<TParameter, TResult> source, TParameter parameter) :
			this(source.Get, parameter) {}

		public FixedSource(Func<TParameter, TResult> source, TParameter parameter)
		{
			_source    = source;
			_parameter = parameter;
		}

		public TResult Get() => _source(_parameter);
	}
}