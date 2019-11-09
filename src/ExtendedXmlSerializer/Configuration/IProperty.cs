using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	public interface IProperty<T> : ISource<T>
	{
		void Assign(T value);
	}
}