namespace ExtendedXmlSerializer.ContentModel.Identification
{
	/// <summary>
	/// Used to store identities so that they are unique, and retrieve them during serialization and deserialization.
	/// </summary>
	public interface IIdentityStore
	{
		/// <summary>
		/// Gets the identity located with the provided input.
		/// </summary>
		/// <param name="name">Name of the identity.</param>
		/// <param name="identifier">Namespace of the identity.</param>
		/// <returns></returns>
		IIdentity Get(string name, string identifier);
	}
}