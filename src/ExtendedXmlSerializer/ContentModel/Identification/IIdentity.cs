namespace ExtendedXmlSerializer.ContentModel.Identification
{
	/// <summary>
	/// Root-level component that marks the identity of an object.  An identity has a namespace and name.  Used together these produce a unique identity that is used to identify objects during serialization and deserialization.
	/// </summary>
	public interface IIdentity
	{

		/// <summary>
		/// The identifier for the identity, usually a namespace or URI.
		/// </summary>
		string Identifier { get; }

		/// <summary>
		/// The identity's name.
		/// </summary>
		string Name { get; }
	}
}