namespace ExtendedXmlSerializer.ContentModel.Members
{
	/// <summary>
	/// A member-specific serializer that provides additional member-specific functionality and information.
	/// </summary>
	public interface IMemberSerializer : ISerializer
	{
		/// <summary>
		/// The member profile associated with this serializer.
		/// </summary>
		IMember Profile { get; }

		/// <summary>
		/// Member access for querying a value for serialization, as well as setting/reading values.
		/// </summary>
		IMemberAccess Access { get; }
	}
}