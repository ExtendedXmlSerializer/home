using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	class SingletonSource<T> : ISource<T>
	{
		readonly Lazy<T> _source;

		public SingletonSource(Func<T> source) : this(new Lazy<T>(source)) {}

		SingletonSource(Lazy<T> source)
		{
			_source = source;
		}

		public T Get() => _source.Value;
	}
}