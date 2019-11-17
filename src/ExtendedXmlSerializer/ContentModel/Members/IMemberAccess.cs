using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// Used during serialization and deserialization to get and set values, respectively.  Also used to determine if a
	/// member should be emitted.
	/// </summary>
	public interface IMemberAccess : ISpecification<object>
	{
		/// <summary>
		/// Used during writing to determine if the value should be emitted based on its containing instance value.
		/// </summary>
		ISpecification<object> Instance { get; }

		/// <summary>
		/// Gets the member value based on the provided instance.
		/// </summary>
		/// <param name="instance">The containing instance of the member.</param>
		/// <returns></returns>
		object Get(object instance);

		/// <summary>
		/// Assigns the provided value based on the provided containing instance.
		/// </summary>
		/// <param name="instance">The containing instance that contains the member.</param>
		/// <param name="value">The value to assign the member.</param>
		void Assign(object instance, object value);
	}
}