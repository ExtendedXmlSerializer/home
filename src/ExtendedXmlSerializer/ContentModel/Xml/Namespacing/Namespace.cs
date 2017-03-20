namespace ExtendedXmlSerializer.ContentModel.Xml.Namespacing
{
	public struct Namespace : IIdentity
	{
		public Namespace(string name) : this(name, string.Empty) {}

		public Namespace(string name, string identifier)
		{
			Name = name;
			Identifier = identifier;
		}

		public string Name { get; }
		public string Identifier { get; }
	}
}