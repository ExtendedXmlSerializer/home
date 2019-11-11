using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Instances;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.IO;
using System.Xml;
// ReSharper disable TooManyArguments

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		/// <summary>
		/// Enables the use of target instances to read values into when deserialization occurs.  <seealso cref="UsingTarget{T}" />
		/// </summary>
		/// <param name="this">The configuration container to configure.</param>
		/// <returns>The configured configuration container.</returns>
		public static IConfigurationContainer AllowTargetInstances(this IConfigurationContainer @this)
			=> @this.Root.With<ExistingInstanceExtension>().Return(@this);

		/// <summary>
		/// Specifies an existing reference to use as target for deserialization of values.  Read values during
		/// deserialization will be read and assigned into the provided target instance.
		///
		/// NOTICE: Be sure to call <see cref="AllowExistingInstances"/> when configuring the container before using this method.
		/// </summary>
		/// <typeparam name="T">The instance type.</typeparam>
		/// <param name="this">The serializer</param>
		/// <param name="instance">The instance to deserialize </param>
		/// <returns>A ReferencedDeserialization&lt;T&gt; context.</returns>
		/// <seealso href="https://github.com/ExtendedXmlSerializer/ExtendedXmlSerializer/issues/230"/>
		public static ReferencedDeserialization<T> UsingTarget<T>(this IExtendedXmlSerializer @this, T instance)
			where T : class
			=> new ReferencedDeserialization<T>(@this, instance);

		public static IConfigurationContainer EnableThreadProtection(this IConfigurationContainer @this)
			=> @this.Extend(ThreadProtectionExtension.Default);

		public static IConfigurationContainer EnableMemberExceptionHandling(this IConfigurationContainer @this)
			=> @this.Extend(MemberExceptionHandlingExtension.Default);

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

		#region Obsolete

		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, string data) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(data);

		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, XmlReaderSettings settings,
		                               string data) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(settings, data);

		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, Stream stream) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(stream);

		[Obsolete("Use IExtendedXmlSerializer.UsingTarget.Deserialize instead.")]
		public static T Deserialize<T>(this IExtendedXmlSerializer @this, T existing, XmlReaderSettings settings,
		                               Stream stream) where T : class
			=> @this.UsingTarget(existing)
			        .Deserialize(settings, stream);

		[Obsolete("Use AllowTargetInstances instead.")]
		public static IConfigurationContainer AllowExistingInstances(this IConfigurationContainer @this)
			=> @this.AllowTargetInstances();

		[Obsolete(
			"This method is being deprecated.  Please use ConfigurationContainer.WithUnknownContent.Call instead.")]
		public static IConfigurationContainer EnableUnknownContentHandling(this IConfigurationContainer @this,
		                                                                   Action<IFormatReader> onMissing)
			=> @this.WithUnknownContent()
			        .Call(onMissing);

		#endregion
	}
}