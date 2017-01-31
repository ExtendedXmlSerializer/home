using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion
{
	public interface IConverter : IParameterizedSource<IReader, object>, IClassification
	{
		void Write(IWriter writer, object instance);
	}
}