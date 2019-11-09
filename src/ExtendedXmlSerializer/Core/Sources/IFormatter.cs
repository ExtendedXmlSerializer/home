namespace ExtendedXmlSerializer.Core.Sources
{
	public interface IFormatter<in T> : IParameterizedSource<T, string> {}
}