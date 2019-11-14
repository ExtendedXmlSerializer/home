namespace ExtendedXmlSerializer.Configuration
{
	public sealed class MemberRegistrationContext<T, TMember>
	{
		readonly IMemberConfiguration<T, TMember> _member;

		public MemberRegistrationContext(IMemberConfiguration<T, TMember> member) => _member = member;

		public MemberSerializationRegistrationContext<T, TMember> Serializer()
			=> new MemberSerializationRegistrationContext<T, TMember>(_member);
	}
}