using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// A general purpose (attached)
	/// property object that stores information and can in turn be used and stored by components that use
	/// it.  This is considered internal code and not to be used by external applications.
	/// </summary>
	/// <typeparam name="T">The property's value type.</typeparam>
	public interface IProperty<T> : ISource<T>
	{
		/// <summary>
		/// Used to assign a value for the property.
		/// </summary>
		/// <param name="value"></param>
		void Assign(T value);
	}
}