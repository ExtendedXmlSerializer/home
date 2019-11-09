using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class Activator : DelegatedSource<object>, IActivator
	{
		public Activator(Func<object> source) : base(source) {}
	}
}