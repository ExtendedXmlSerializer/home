using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.Core.Sources
{
    public static class Extensions
    {
        public static ISource<TResult> Fix<TParameter, TResult>(this IParameterizedSource<TParameter, TResult> @this,
                                                                TParameter parameter)
            => new FixedSource<TParameter, TResult>(@this, parameter);
        public static ISource<T> Singleton<T>(this ISource<T> @this) => new SingletonSource<T>(@this.Get);

        public static Func<T> ToDelegate<T>(this ISource<T> @this) => @this.Get;
    }

    public class FixedSource<TParameter, TResult> : ISource<TResult>
    {
        readonly IParameterizedSource<TParameter, TResult> _source;
        private readonly TParameter _parameter;

        public FixedSource(IParameterizedSource<TParameter, TResult> source, TParameter parameter)
        {
            _source = source;
            _parameter = parameter;
        }

        public TResult Get() => _source.Get(_parameter);
    }
}