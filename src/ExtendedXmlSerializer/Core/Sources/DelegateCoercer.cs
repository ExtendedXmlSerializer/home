using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	sealed class DelegateCoercer<T> : IParameterizedSource<Func<T>, T>
    {
	    public static DelegateCoercer<T> Default { get; } = new DelegateCoercer<T>();
	    DelegateCoercer() {}

	    public T Get(Func<T> parameter) => parameter.Invoke();
    }
}
