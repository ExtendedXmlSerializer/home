using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IMemberContext : IElementContext, IName
	{
		object Get(object instance);

		void Assign(object instance, object value);
	}
}