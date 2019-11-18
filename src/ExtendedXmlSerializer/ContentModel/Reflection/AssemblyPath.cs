namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	readonly struct AssemblyPath
	{
		public AssemblyPath(string @namespace, string path)
		{
			Namespace = @namespace;
			Path      = path;
		}

		public string Namespace { get; }
		public string Path { get; }
	}
}