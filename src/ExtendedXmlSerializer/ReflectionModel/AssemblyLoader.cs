using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AssemblyLoader : IAssemblyLoader
	{
		readonly ImmutableArray<Assembly> _loaded;
		public static AssemblyLoader Default { get; } = new AssemblyLoader();

		AssemblyLoader() : this(AssemblyProvider.Default.Get()
		                                        .ToImmutableArray()) {}

		AssemblyLoader(ImmutableArray<Assembly> loaded) => _loaded = loaded;

		public Assembly Get(string parameter)
		{
			try
			{
				return Assembly.Load(new AssemblyName(parameter));
			}
			catch (FileNotFoundException)
			{
				var length = _loaded.Length;
				for (var i = 0; i < length; i++)
				{
					var assembly = _loaded[i];
					if (assembly.GetName()
					            .Name == parameter)
					{
						return assembly;
					}
				}

				throw;
			}
		}
	}
}