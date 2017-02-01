namespace ExtendedXmlSerialization.Conversion.Elements
{
	public abstract class ContainerBase : DecoratedConverter
	{
		protected ContainerBase(IConverter body) : base(body) {}

		protected abstract IElement Element { get; }

		public override void Emit(IWriter writer, object instance)
		{
			using (writer.Emit(Element))
			{
				base.Emit(writer, instance);
			}
		}
	}

	public class Container : ContainerBase
	{
		public Container(IElement element, IConverter body) : base(body)
		{
			Element = element;
		}

		protected override IElement Element { get; }
	}
}