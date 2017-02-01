namespace ExtendedXmlSerialization.Conversion
{
	public class DecoratedConverter : ConverterBase
	{
		readonly IConverter _context;

		public DecoratedConverter(IConverter context)
		{
			_context = context;
		}

		public override void Emit(IWriter writer, object instance) => _context.Emit(writer, instance);
		public override object Get(IReader reader) => _context.Get(reader);
	}
}