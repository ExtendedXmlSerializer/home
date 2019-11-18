namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// A base component used by configuration components to assume shared functionality.
	/// </summary>
	public abstract class ContextBase : IContext
	{
		/// <inheritdoc />
		protected ContextBase(IContext parent) : this(parent.Root, parent) {}

		/// <inheritdoc />
		protected ContextBase(IRootContext root, IContext parent)
		{
			Root   = root;
			Parent = parent;
		}

		/// <summary>
		/// The root context.
		/// </summary>
		public IRootContext Root { get; }

		/// <summary>
		/// Represents this context's parent.
		/// </summary>
		public IContext Parent { get; }

		/// <inheritdoc cref="IRootContext.Create" />
		public IExtendedXmlSerializer Create() => Root.Create();
	}
}