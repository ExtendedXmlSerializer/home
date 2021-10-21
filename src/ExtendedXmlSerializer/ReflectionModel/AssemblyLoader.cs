using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class AssemblyLoader : IAssemblyLoader
	{
		public static AssemblyLoader Default { get; } = new AssemblyLoader();

		AssemblyLoader() : this(AssemblyPath.Default.Get, AssemblyProvider.Default.Get().ToImmutableArray()) {}

		AssemblyLoader(Func<string, string> path, ImmutableArray<Assembly> loaded)
		{
			_path   = path;
			_loaded = loaded;
		}

		readonly Func<string, string>     _path;
		readonly ImmutableArray<Assembly> _loaded;

		public Assembly Get(string parameter)
		{
			try
			{
				return Assembly.Load(new AssemblyName(parameter));
			}
			catch (FileNotFoundException)
			{
				try
				{
					return Assembly.LoadFile(_path(parameter));
				}
				catch (Exception error)
				{
					var length = _loaded.Length;
					for (var i = 0; i < length; i++)
					{
						var assembly = _loaded[i];
						if (assembly.GetName().Name == parameter)
						{
							return assembly;
						}
					}

					throw new InvalidOperationException($"Could not load assembly '{parameter}'.  Are you sure it exists?", error);
				}
			}
		}
	}
}