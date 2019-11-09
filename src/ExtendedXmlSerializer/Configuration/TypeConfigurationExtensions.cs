namespace ExtendedXmlSerializer.Configuration
{
	static class TypeConfigurationExtensions
	{
		public static IInternalTypeConfiguration AsInternal(this ITypeConfiguration @this)
		{
			return (IInternalTypeConfiguration)@this;
		}
	}
}