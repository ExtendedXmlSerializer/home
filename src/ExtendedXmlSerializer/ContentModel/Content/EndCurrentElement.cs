using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class EndCurrentElement<T> : IWriter<T>
	{
		public static EndCurrentElement<T> Default { get; } = new EndCurrentElement<T>();

		EndCurrentElement() {}

		public void Write(IFormatWriter writer, T instance) => writer.EndCurrent();
	}
}