using ExtendedXmlSerializer.Configuration;

namespace ExtendedXmlSerializer.Tests.Support
{
	static class Extensions
	{
		public static T Cycle<T>(this IExtendedXmlSerializer @this, T instance)
			=> @this.Deserialize<T>(@this.Serialize(instance));

		/*public static SerializationSupport ForTesting(this IConfigurationItem @this) => new SerializationSupport(@this.Configuration);*/

		public static SerializationSupport ForTesting(this IConfigurationContainer @this)
			=> new SerializationSupport(@this);

		public static SerializationSupport ForTesting(this IExtendedXmlSerializer @this)
			=> new SerializationSupport(@this);
	}
}