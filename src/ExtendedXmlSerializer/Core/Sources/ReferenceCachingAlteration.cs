using System;
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class ReferenceCachingAlteration<TParameter, TResult> : IAlteration<Func<TParameter, TResult>>
		where TParameter : class where TResult : class
	{
		public static ReferenceCachingAlteration<TParameter, TResult> Default { get; } =
			new ReferenceCachingAlteration<TParameter, TResult>();

		ReferenceCachingAlteration() {}

		public Func<TParameter, TResult> Get(Func<TParameter, TResult> parameter)
			=> new ReferenceCache<TParameter, TResult>(
			                                           new
				                                           ConditionalWeakTable<TParameter, TResult>.
				                                           CreateValueCallback(parameter)).Get;
	}
}