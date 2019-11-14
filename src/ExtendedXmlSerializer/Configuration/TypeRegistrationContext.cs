namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Used to establish a content regstration context for a type configuration.  Using this context you can further
	/// establish registration contexts for serializers or converters.
	/// </summary>
	/// <typeparam name="T">The type under configuration.</typeparam>
	public sealed class TypeRegistrationContext<T>
	{
		readonly ITypeConfiguration<T> _configuration;

		public TypeRegistrationContext(ITypeConfiguration<T> configuration) => _configuration = configuration;

		/// <summary>
		/// Establishes a Serializer-registration context.
		/// </summary>
		/// <returns>A context to perform operations on serializer registrations for the captured type configuration.</returns>
		public TypeSerializationRegistrationContext<T> Serializer()
			=> new TypeSerializationRegistrationContext<T>(_configuration);

		/// <summary>
		/// Establishes a Converter-registration context.
		/// </summary>
		/// <returns>A context to perform operations on converter registrations for the captured type configuration.</returns>
		public TypeConverterRegistrationContext<T> Converter()
			=> new TypeConverterRegistrationContext<T>(_configuration);
	}
}