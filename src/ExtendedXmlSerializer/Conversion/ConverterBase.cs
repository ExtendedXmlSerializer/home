namespace ExtendedXmlSerialization.Conversion
{
	public abstract class ConverterBase : IConverter
	{
		public abstract void Emit(IWriter writer, object instance);
		
		public abstract object Get(IReader reader);
	}

	public abstract class ConverterBase<T> : ConverterBase
	{
		public override void Emit(IWriter writer, object instance) => Emit(writer, (T) instance);

		public abstract void Emit(IWriter writer, T instance);
	}
}