namespace ExtendedXmlSerializer.Configuration
{
	static class MemberConfigurationExtensions
	{
		public static IInternalMemberConfiguration AsInternal(this IMemberConfiguration @this)
		{
			return (IInternalMemberConfiguration)@this;
		}
	}
}