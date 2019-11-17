namespace ExtendedXmlSerializer.Configuration
{
	public abstract class ContextBase : IContext
	{
		protected ContextBase(IContext parent) : this(parent.Root, parent) {}

		protected ContextBase(IRootContext root, IContext parent)
		{
			Root   = root;
			Parent = parent;
		}

		public IRootContext Root { get; }
		public IContext Parent { get; }

		public IExtendedXmlSerializer Create() => Root.Create();
	}
}