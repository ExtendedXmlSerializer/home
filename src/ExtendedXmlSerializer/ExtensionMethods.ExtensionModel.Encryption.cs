using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Encryption;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>
		/// Encrypts the specified member with the default encryption, which is a base-64 string.
		/// </summary>
		/// <typeparam name="T">The containing type of the member.</typeparam>
		/// <typeparam name="TMember">The member type.</typeparam>
		/// <param name="this">The member to configure.</param>
		/// <returns>The configured MemberConfiguration.</returns>
		public static IMemberConfiguration<T, TMember> Encrypt<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<EncryptionExtension>()
			        .Registered.Apply(@this.GetMember())
			        .Return(@this);

		/// <summary>
		/// Configures a container for default encryption, which is base-64.  Every registered converter will be wrapped with a converter which will further encrypt its reading and writing.
		/// </summary>
		/// <param name="this">The container to configure for encryption.</param>
		/// <returns>A configured IConfigurationContainer.</returns>
		public static IConfigurationContainer UseEncryptionAlgorithm(this IConfigurationContainer @this)
			=> @this.UseEncryptionAlgorithm(EncryptionConverterAlteration.Default);

		/// <summary>
		/// Configures a container for encryption with the specified encryption component.  Every registered converter will be wrapped with a converter which will further encrypt its reading and writing.
		/// </summary>
		/// <param name="this">The container to configure for encryption.</param>
		/// <param name="encryption">The encryption with which to encrypt and decrypt data.</param>
		/// <returns>A configured IConfigurationContainer.</returns>
		public static IConfigurationContainer UseEncryptionAlgorithm(this IConfigurationContainer @this,
		                                                             IEncryption encryption)
			=> @this.UseEncryptionAlgorithm(new EncryptionConverterAlteration(encryption));

		/// <summary>
		/// Configures a container for encryption with the specified converter alteration.  Every registered converter will be altered by the provided alteration.
		/// </summary>
		/// <param name="this">The container which to configure for encryption.</param>
		/// <param name="parameter">The alteration with which to alter each converter in the configuration container.</param>
		/// <returns>A configured IConfigurationContainer.</returns>
		public static IConfigurationContainer UseEncryptionAlgorithm(this IConfigurationContainer @this,
		                                                             IAlteration<IConverter> parameter)
			=> @this.Root
			        .With<EncryptionExtension>()
			        .IsSatisfiedBy(parameter)
				   ? @this
				   : @this.Extend(new EncryptionExtension(parameter));
	}
}