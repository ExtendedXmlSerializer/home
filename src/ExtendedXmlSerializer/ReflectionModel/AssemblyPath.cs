using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Runtime.InteropServices;

namespace ExtendedXmlSerializer.ReflectionModel
{
	/// <summary>
	/// Attribution: https://stackoverflow.com/a/6131116/10340424
	/// </summary>
	sealed class AssemblyPath : IParameterizedSource<string, string>
	{
		public static AssemblyPath Default { get; } = new AssemblyPath();

		AssemblyPath() {}

		public string Get(string parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			var finalName = parameter;
			var aInfo     = new AssemblyInfo {cchBuf = 1024}; // should be fine...
			aInfo.currentAssemblyPath = new string('\0', aInfo.cchBuf);

			var hr = CreateAssemblyCache(out var ac, 0);
			if (hr >= 0)
			{
				hr = ac.QueryAssemblyInfo(0, finalName, ref aInfo);
				if (hr < 0)
				{
					return null;
				}
			}

			return aInfo.currentAssemblyPath;
		}

		[DllImport("fusion.dll")]
		static extern int CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
		interface IAssemblyCache
		{
			void Reserved0();

			[PreserveSig]
			int QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName,
			                      ref AssemblyInfo assemblyInfo);
		}

		[StructLayout(LayoutKind.Sequential)]
		struct AssemblyInfo
		{
			readonly                                 int    cbAssemblyInfo;
			readonly                                 int    assemblyFlags;
			readonly                                 long   assemblySizeInKB;
			[MarshalAs(UnmanagedType.LPWStr)] public string currentAssemblyPath;
			public                                   int    cchBuf; // size of path buf.
		}
	}
}