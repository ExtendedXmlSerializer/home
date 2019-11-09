using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel
{
	public interface IWriter : IWriter<object> {}

	public interface IWriter<in T>
	{
		void Write(IFormatWriter writer, T instance);
	}
}