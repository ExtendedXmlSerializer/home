namespace ExtendedXmlSerialization.Conversion.Names
{
	public abstract class ContainerBase : DecoratedConverter
	{
		protected ContainerBase(IConverter body) : base(body) {}

		protected abstract IName Name { get; }

		public override void Emit(IWriter writer, object instance)
		{
			using (writer.Emit(Name))
			{
				base.Emit(writer, instance);
			}
		}
	}

	public class Container : ContainerBase
	{
		public Container(IName name, IConverter body) : base(body)
		{
			Name = name;
		}

		protected override IName Name { get; }
	}
}