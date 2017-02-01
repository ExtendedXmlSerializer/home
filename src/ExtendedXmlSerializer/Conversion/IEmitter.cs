namespace ExtendedXmlSerialization.Conversion
{
	public interface IEmitter
	{
		void Emit(IWriter writer, object instance);
	}
}