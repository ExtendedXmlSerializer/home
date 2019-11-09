using System;

namespace ExtendedXmlSerializer.Core.Sources
{
	class DelegatedAlteration<T> : IAlteration<T>
	{
		readonly Func<T, T> _alteration;

		public DelegatedAlteration(Func<T, T> alteration) => _alteration = alteration;

		public T Get(T parameter) => _alteration(parameter);
	}
}