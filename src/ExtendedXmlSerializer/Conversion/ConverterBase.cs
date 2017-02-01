using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	public abstract class ConverterBase : IConverter
	{
		protected ConverterBase(TypeInfo classification)
		{
			Classification = classification;
		}

		public abstract void Emit(IWriter writer, object instance);
		public TypeInfo Classification { get; }
		public abstract object Get(IReader reader);
	}

	public abstract class ConverterBase<T> : ConverterBase
	{
		protected ConverterBase(TypeInfo classification) : base(classification) {}

		public override void Emit(IWriter writer, object instance) => Emit(writer, (T) instance);

		public abstract void Emit(IWriter writer, T instance);
	}
}