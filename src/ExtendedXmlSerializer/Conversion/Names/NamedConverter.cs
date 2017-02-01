namespace ExtendedXmlSerialization.Conversion.Names
{
	public class NamedConverter : NamedConverter<IName>
	{
		public NamedConverter(IName name, IConverter body) : base(name, body) {}
	}

	public class NamedConverter<T> : DecoratedConverter where T : IName
	{
		public NamedConverter(T name, IConverter body) : base(body)
		{
			Name = name;
		}

		protected T Name { get; }

		public override void Emit(IWriter writer, object instance)
		{
			using (writer.Emit(Name))
			{
				base.Emit(writer, instance);
			}
		}
	}

}