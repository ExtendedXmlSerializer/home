using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Encryption;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IMemberConfiguration<T, TMember> Encrypt<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<EncryptionExtension>()
			     .Registered.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IConfigurationContainer UseEncryptionAlgorithm(this IConfigurationContainer @this)
			=> UseEncryptionAlgorithm(@this, EncryptionConverterAlteration.Default);

		public static IConfigurationContainer UseEncryptionAlgorithm(this IConfigurationContainer @this,
		                                                             IEncryption encryption)
			=> UseEncryptionAlgorithm(@this, new EncryptionConverterAlteration(encryption));

		public static IConfigurationContainer UseEncryptionAlgorithm(this IConfigurationContainer @this,
		                                                             IAlteration<IConverter> parameter)
			=> @this.Root
			        .With<EncryptionExtension>()
			        .IsSatisfiedBy(parameter)
				   ? @this
				   : @this.Extend(new EncryptionExtension(parameter));
	}
}
