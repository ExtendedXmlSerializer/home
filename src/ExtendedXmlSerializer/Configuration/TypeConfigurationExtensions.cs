namespace ExtendedXmlSerializer.Configuration
{
	static class TypeConfigurationExtensions
	{
		public static IInternalTypeConfiguration AsInternal(this ITypeConfiguration @this)
			=> (IInternalTypeConfiguration)@this;
	}
}