using ExtendedXmlSerialization.Conversion.Elements;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMember : IConverter, IDisplayAware
	{
		object Get(object instance);

		void Assign(object instance, object value);
	}
}