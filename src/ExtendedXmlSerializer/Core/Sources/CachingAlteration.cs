using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class CachingAlteration<TParameter, TResult> : IAlteration<Func<TParameter, TResult>>
	{
		public static CachingAlteration<TParameter, TResult> Default { get; } =
			new CachingAlteration<TParameter, TResult>();

		CachingAlteration() {}

		public Func<TParameter, TResult> Get(Func<TParameter, TResult> parameter)
			=> new Cache<TParameter, TResult>(parameter).Get;
	}
}