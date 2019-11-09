using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	struct Key : IIdentity
	{
		public Key(string name) : this(name, string.Empty) {}

		public Key(string name, string identifier)
		{
			Name       = name;
			Identifier = identifier;
		}

		public string Identifier { get; }
		public string Name { get; }
	}
}