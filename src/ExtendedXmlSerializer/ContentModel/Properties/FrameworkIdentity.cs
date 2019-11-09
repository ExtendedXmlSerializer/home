using ExtendedXmlSerializer.ContentModel.Identification;

namespace ExtendedXmlSerializer.ContentModel.Properties
{
	sealed class FrameworkIdentity : IIdentity
	{
		public FrameworkIdentity(string name) : this(name, Defaults.Identifier) {}

		public FrameworkIdentity(string name, string identifier)
		{
			Name       = name;
			Identifier = identifier;
		}

		public string Identifier { get; }

		public string Name { get; }
	}
}