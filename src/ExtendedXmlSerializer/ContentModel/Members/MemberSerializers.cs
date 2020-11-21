using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerializers : IMemberSerializers
	{
		readonly IMemberAccessors         _accessors;
		readonly IAttributeSpecifications _runtime;
		readonly IMemberConverters        _converters;
		readonly IMemberContents          _content;
		readonly IIdentityStore           _identities;
		readonly ITypes                   _types;
		readonly IIdentifiers             _identifiers;

		// ReSharper disable once TooManyDependencies
		public MemberSerializers(IAttributeSpecifications runtime, IMemberAccessors accessors,
		                         IMemberConverters converters, IMemberContents content, IIdentityStore identities,
		                         ITypes types, IIdentifiers identifiers)
		{
			_runtime     = runtime;
			_accessors   = accessors;
			_converters  = converters;
			_content     = content;
			_identities  = identities;
			_types       = types;
			_identifiers = identifiers;
		}

		public IMemberSerializer Get(IMember parameter)
		{
			var converter = _converters.Get(parameter);
			var access    = _accessors.Get(parameter);
			var result    = converter != null ? Property(converter, parameter, access) : Content(parameter, access);
			return result;
		}

		IMemberSerializer Property(IConverter converter, IMember profile, IMemberAccess access)
		{
			var alteration = new DelegatedAlteration<object>(access.Get);
			var serializer = new ConverterProperty<object>(converter, profile).Adapt();
			var member     = new MemberSerializer(profile, access, serializer, new MemberWriter(access, serializer));
			var runtime    = _runtime.Get(profile.Metadata);
			var property   = (IMemberSerializer)new PropertyMemberSerializer(member);
			return runtime != null
				       ? new RuntimeSerializer(new AlteredSpecification<object>(alteration, runtime),
				                               property, Content(profile, access))
				       : property;
		}

		bool IsMember(IMember profile)
			=> CollectionItemTypeLocator.Default.Get(profile.MemberType)?.Name == profile.Name ||
			   _types.Get(_identities.Get(profile.Name, _identifiers.Get(profile.Metadata.ReflectedType))) != null;

		IMemberSerializer Content(IMember profile, IMemberAccess access)
		{
			var identity = new Identity<object>(profile);
			var composite = IsMember(profile)
				                ? (IWriter<object>)new MemberPropertyWriter(identity)
				                : identity;
			var start  = composite.Adapt();
			var body   = _content.Get(profile);
			var writer = new MemberWriter(access, new Enclosure(start, body));
			var result = new MemberSerializer(profile, access, body, writer);
			return result;
		}
	}
}