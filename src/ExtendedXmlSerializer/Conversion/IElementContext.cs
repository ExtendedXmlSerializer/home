using ExtendedXmlSerialization.Conversion.Model;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IElementContext : IClassification
	{
		void Emit(IEmitter emitter, object instance);

		object Yield(IYielder yielder);
	}
}