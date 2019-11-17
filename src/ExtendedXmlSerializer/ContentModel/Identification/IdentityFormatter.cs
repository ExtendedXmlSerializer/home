using ExtendedXmlSerializer.Core.Sources;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Identification
{
	/// <summary>
	/// Formats a provided identity into its text equivalent.
	/// </summary>
	public sealed class IdentityFormatter : IdentityFormatter<IIdentity>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public new static IdentityFormatter Default { get; } = new IdentityFormatter();

		IdentityFormatter() {}
	}

	/// <summary>
	/// Formats a provided identity into its text equivalent.
	/// </summary>
	/// <typeparam name="T">The identity type.</typeparam>
	public class IdentityFormatter<T> : IFormatter<T> where T : IIdentity
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static IdentityFormatter<T> Default { get; } = new IdentityFormatter<T>();


		/// <summary>
		/// Creates a new instance.
		/// </summary>
		protected IdentityFormatter() {}

		/// <inheritdoc />
		public string Get(T parameter) => XmlQualifiedName.ToString(parameter.Name, parameter.Identifier);
	}
}