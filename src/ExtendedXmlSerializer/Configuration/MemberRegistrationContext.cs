namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Establishes a context to apply registrations to a member configuration.
	/// </summary>
	/// <typeparam name="T">The member's containing type.</typeparam>
	/// <typeparam name="TMember">The member's value type.</typeparam>
	public sealed class MemberRegistrationContext<T, TMember>
	{
		readonly IMemberConfiguration<T, TMember> _member;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="member">The member configuration to configure.</param>
		public MemberRegistrationContext(IMemberConfiguration<T, TMember> member) => _member = member;

		/// <summary>
		/// Establishes a new context to configure the serializer registration for this context's member configuration.
		/// </summary>
		/// <returns>A context to configure serializer registration.</returns>
		public MemberSerializationRegistrationContext<T, TMember> Serializer()
			=> new MemberSerializationRegistrationContext<T, TMember>(_member);
	}
}