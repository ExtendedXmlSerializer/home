using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content;
using System;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Establishes a configuration context that allows operations to be performed against a provided configuration
	/// container.  These operations configure the serializer(s) created by the container so that they handle unknown
	/// content -- content that isn't recognized in the deserialized document -- in the configured manner.
	/// </summary>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/271" />
	public sealed class UnknownContentContext
	{
		readonly IConfigurationContainer _container;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="container">The configuration container under configuration.</param>
		public UnknownContentContext(IConfigurationContainer container) => _container = container;

		/// <summary>
		/// Configures created serializers to continue reading if unknown content is encountered.
		/// </summary>
		/// <returns>The configured configuration container.</returns>
		public IConfigurationContainer Continue() => Call(ContinueOnUnknownContent.Default.Execute);

		/// <summary>
		/// Configures created serializers to throw upon encountering unknown content.
		/// </summary>
		/// <returns>The configured configuration container.</returns>
		public IConfigurationContainer Throw() => Call(ThrowOnUnknownContent.Default.Execute);

		/// <summary>
		/// Configures created serializers to call the provided delegate upon encountering unknown content.
		/// </summary>
		/// <param name="callback">The delegate to invoke when unknown content is encountered.</param>
		/// <returns>The configured configuration container.</returns>
		public IConfigurationContainer Call(Action<IFormatReader> callback)
			=> _container.Extend(new UnknownContentHandlingExtension(callback));
	}
}