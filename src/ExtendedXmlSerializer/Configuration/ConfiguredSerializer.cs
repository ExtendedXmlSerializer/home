namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Defines, instantiates, and applies a profile to a serializer, which is also instantiated.  A profile is a stored
	/// preset configuration that can be applied to a serializer.
	/// </summary>
	/// <typeparam name="T">The profile type.</typeparam>
	public class ConfiguredSerializer<T> : DecoratedSerializer, IExtendedXmlSerializer
		where T : class, IConfigurationProfile
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ConfiguredSerializer() : this(ConfiguredContainer.New<T>().Create()) {}

		/// <summary>
		/// Creates a new instance with the provided serializer.
		/// </summary>
		/// <param name="serializer">The serializer to decorate.</param>
		public ConfiguredSerializer(IExtendedXmlSerializer serializer) : base(serializer) {}
	}
}