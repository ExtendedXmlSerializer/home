using System.Xml;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	public sealed class IdentityFormatter : IdentityFormatter<IIdentity>
	{
		public new static IdentityFormatter Default { get; } = new IdentityFormatter();

		IdentityFormatter() {}
	}

	public class IdentityFormatter<T> : IFormatter<T> where T : IIdentity
	{
		public static IdentityFormatter<T> Default { get; } = new IdentityFormatter<T>();

		protected IdentityFormatter() {}

		public string Get(T parameter) => XmlQualifiedName.ToString(parameter.Name, parameter.Identifier);
	}
}