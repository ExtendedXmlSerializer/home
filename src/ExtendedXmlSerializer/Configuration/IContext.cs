namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// A general context component that is used by the configuration API.  Should not be used by external applications.
	/// </summary>
	public interface IContext
	{
		/// <summary>
		/// Provides access to the root context.
		/// </summary>
		IRootContext Root { get; }

		/// <summary>
		/// Provides access to the context's parent context.
		/// </summary>
		IContext Parent { get; }
	}
}