using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	struct TypePartition
	{
		public TypePartition(Assembly assembly, string @namespace, string name)
		{
			Assembly  = assembly;
			Namespace = @namespace;
			Name      = name;
		}

		public Assembly Assembly { get; }
		public string Namespace { get; }
		public string Name { get; }
	}
}