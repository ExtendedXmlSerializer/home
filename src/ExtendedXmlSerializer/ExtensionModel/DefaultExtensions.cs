using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class DefaultExtensions : ItemsBase<ISerializerExtension>
	{
		public static DefaultExtensions Default { get; } = new DefaultExtensions();

		DefaultExtensions()
			: this(DefaultMetadataSpecification.Default, DeclaredMemberNames.Default, DefaultMemberOrder.Default) {}

		readonly IMetadataSpecification                _metadata;
		readonly INames                                _defaultNames;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		public DefaultExtensions(IMetadataSpecification metadata, INames defaultNames,
		                         IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_metadata           = metadata;
			_defaultNames       = defaultNames;
			_defaultMemberOrder = defaultMemberOrder;
		}

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
			yield return new CustomSerializationExtension();
			yield return CachingExtension.Default;
		}
	}
}