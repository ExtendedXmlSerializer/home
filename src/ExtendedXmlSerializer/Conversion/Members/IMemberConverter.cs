using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public interface IMemberConverter : IConverter, IName
	{
		object Get(object instance);

		void Assign(object instance, object value);
	}
}