using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMember : IConverter, IElement
	{
		object Get(object instance);

		void Assign(object instance, object value);
	}
}