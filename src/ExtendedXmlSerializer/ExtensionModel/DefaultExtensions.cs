using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// Compiles a list of default extensions used in the configuration container.
	/// </summary>
	public sealed class DefaultExtensions : ItemsBase<ISerializerExtension>
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static DefaultExtensions Default { get; } = new DefaultExtensions();

		DefaultExtensions()
			: this(DefaultMetadataSpecification.Default, DeclaredMemberNames.Default, DefaultMemberOrder.Default) {}

		readonly IMetadataSpecification                _metadata;
		readonly INames                                _defaultNames;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="metadata">The metadata specification used to create a <see cref="AllowedMembersExtension"/>.</param>
		/// <param name="defaultNames">The default names selector for element and attribute names.</param>
		/// <param name="defaultMemberOrder">The order selector to use when a <see cref="MemberInfo"/> is encountered and
		/// selected.</param>
		public DefaultExtensions(IMetadataSpecification metadata, INames defaultNames,
		                         IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_metadata           = metadata;
			_defaultNames       = defaultNames;
			_defaultMemberOrder = defaultMemberOrder;
		}

		/// <inheritdoc />
		public override IEnumerator<ISerializerExtension> GetEnumerator()
		{
			yield return new DefaultReferencesExtension();
			yield return Contents.Default;
			yield return ContentModelExtension.Default;
			yield return TypeModelExtension.Default;
			yield return SingletonActivationExtension.Default;
			yield return new XmlSerializationExtension();
			yield return new ConvertersExtension();
			yield return MemberModelExtension.Default;
			yield return new TypeNamesExtension();
			yield return new MemberPropertiesExtension(_defaultNames, _defaultMemberOrder);
			yield return new AllowedMembersExtension(_metadata);
			yield return new AllowedMemberValuesExtension();
			yield return new MemberFormatExtension();
			yield return ImmutableArrayExtension.Default;
			yield return SerializationExtension.Default;
			yield return RecursionAwareExtension.Default;
			yield return NullableStructureAwareExtension.Default;
			yield return new CustomSerializationExtension();
			yield return CachingExtension.Default;
		}
	}
}