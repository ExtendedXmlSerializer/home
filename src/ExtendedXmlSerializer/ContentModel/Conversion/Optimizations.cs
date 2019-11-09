using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	sealed class Optimizations : IAlteration<IConverter>, IOptimizations
	{
		readonly ICollection<Action> _containers = new HashSet<Action>();

		public IConverter Get(IConverter parameter)
		{
			var parse  = Create<string, object>(parameter.Parse);
			var format = Create<object, string>(parameter.Format);
			var result = new Converter<object>(parameter, parse, format);
			return result;
		}

		Func<TParameter, TResult> Create<TParameter, TResult>(Func<TParameter, TResult> source)
		{
			var dictionary = new ConcurrentDictionary<TParameter, TResult>();
			var cache      = new Cache<TParameter, TResult>(source, dictionary);
			_containers.Add(dictionary.Clear);
			var result = cache.ToDelegate();
			return result;
		}

		public void Clear()
		{
			foreach (var container in _containers)
			{
				container.Invoke();
			}
		}
	}
}