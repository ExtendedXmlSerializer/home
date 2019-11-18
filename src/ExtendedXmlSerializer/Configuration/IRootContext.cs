namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Represents a root context object in the configuration API. This is considered internal code and not to be used by
	/// external applications.
	/// </summary>
	public interface IRootContext : IContext, IExtensionCollection
	{
		/// <summary>
		/// A list of type configurations found in this context.
		/// </summary>
		ITypeConfigurations Types { get; }

		/// <summary>
		/// The main event.  Used to create a new serializer and apply any configurations to it.
		/// </summary>
		/// <returns>The configured serializer.</returns>
		IExtendedXmlSerializer Create();
	}
}