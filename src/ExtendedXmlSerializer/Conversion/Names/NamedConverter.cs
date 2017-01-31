using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public class NamedConverter : NamedConverter<IName>
	{
		public NamedConverter(IName name, IConverter body) : base(name, body) {}
	}

	public class NamedConverter<T> : DecoratedConverter where T : IName
	{
		public NamedConverter(T name, IConverter body) : this(name, body, body.Classification) {}

		public NamedConverter(T name, IConverter body, TypeInfo classification) : base(body, classification)
		{
			Name = name;
		}

		protected T Name { get; }

		public override void Write(IWriter writer, object instance)
		{
			using (writer.Emit(Name))
			{
				base.Write(writer, instance);
			}
		}
	}

}