using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AssemblyProvider : IAssemblyProvider
	{
		public static AssemblyProvider Default { get; } = new AssemblyProvider();

		AssemblyProvider() {}

		public IEnumerable<Assembly> Get() => AppDomain.CurrentDomain.GetAssemblies();
	}
}