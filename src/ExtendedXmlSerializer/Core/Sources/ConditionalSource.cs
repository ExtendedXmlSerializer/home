using ExtendedXmlSerializer.Core.Specifications;
using System;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.Core.Sources
{
	/// <exclude />
	public class ConditionalSource<TParameter, TResult> : IParameterizedSource<TParameter, TResult>
	{
		readonly Func<TParameter, bool>    _specification;
		readonly Func<TResult, bool>       _result;
		readonly Func<TParameter, TResult> _source, _fallback;

		/// <exclude />
		public ConditionalSource(ISpecification<TParameter> specification,
		                         IParameterizedSource<TParameter, TResult> source,
		                         IParameterizedSource<TParameter, TResult> fallback)
			: this(specification, AlwaysSpecification<TResult>.Default, source, fallback) {}

		/// <exclude />
		public ConditionalSource(ISpecification<TParameter> specification, ISpecification<TResult> result,
		                         IParameterizedSource<TParameter, TResult> source,
		                         IParameterizedSource<TParameter, TResult> fallback)
			: this(specification.IsSatisfiedBy, result.IsSatisfiedBy, source.Get, fallback.Get) {}

		/// <exclude />
		public ConditionalSource(Func<TParameter, bool> specification, Func<TResult, bool> result,
		                         Func<TParameter, TResult> source)
			: this(specification, result, source, _ => default) {}

		/// <exclude />
		public ConditionalSource(Func<TParameter, bool> specification, Func<TResult, bool> result,
		                         Func<TParameter, TResult> source,
		                         Func<TParameter, TResult> fallback)
		{
			_specification = specification;
			_result        = result;
			_source        = source;
			_fallback      = fallback;
		}

		/// <inheritdoc />
		public TResult Get(TParameter parameter)
		{
			if (_specification(parameter))
			{
				var result = _source(parameter);
				if (_result(result))
				{
					return result;
				}
			}

			return _fallback(parameter);
		}
	}
}