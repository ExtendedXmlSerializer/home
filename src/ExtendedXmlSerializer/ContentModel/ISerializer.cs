namespace ExtendedXmlSerializer.ContentModel
{
	public interface ISerializer : ISerializer<object>, IReader, IWriter {}

	public interface ISerializer<T> : IReader<T>, IWriter<T> {}
}