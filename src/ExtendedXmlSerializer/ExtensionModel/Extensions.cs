using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using System;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public static class Extensions
	{
		public static IConfigurationContainer AllowExistingInstances(this IConfigurationContainer @this)
		{
			@this.Root.With<ExistingInstanceExtension>();
			return @this;
		}

		public static IConfigurationContainer EnableThreadProtection(this IConfigurationContainer @this)
			=> @this.Extend(ThreadProtectionExtension.Default);

		public static IConfigurationContainer EnableMemberExceptionHandling(this IConfigurationContainer @this)
			=> @this.Extend(MemberExceptionHandlingExtension.Default);

		[Obsolete(
			"This method is being deprecated.  Please use ConfigurationContainer.WithUnknownContent.Call instead.")]
		public static IConfigurationContainer EnableUnknownContentHandling(this IConfigurationContainer @this,
		                                                                   Action<IFormatReader> onMissing)
			=> @this.WithUnknownContent()
			        .Call(onMissing);

		/// <summary>
		/// https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/271#issuecomment-550976753
		/// </summary>
		/// <param name="this"></param>
		/// <returns></returns>
		public static UnknownContentContext WithUnknownContent(this IConfigurationContainer @this)
			=> new UnknownContentContext(@this);

		public static T EnableRootInstances<T>(this T @this) where T : IRootContext
		{
			@this.Root.With<RootInstanceExtension>();
			return @this;
		}
	}
}