using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.Configuration
{
	public sealed class MemberSerializationRegistrationContext<T, TMember>
	{
		readonly IMemberConfiguration<T, TMember> _member;

		public MemberSerializationRegistrationContext(IMemberConfiguration<T, TMember> member) => _member = member;

		public IMemberConfiguration<T, TMember> Of<TSerializer>() where TSerializer : ISerializer<TMember>
			=> Of(Support<TSerializer>.Key);

		public IMemberConfiguration<T, TMember> Of(Type serializerType)
			=> Using(new ActivatedSerializer(serializerType, Support<TMember>.Metadata));

		public IMemberConfiguration<T, TMember> Using(ISerializer<TMember> serializer) => Using(serializer.Adapt());

		public IMemberConfiguration<T, TMember> Using(ISerializer serializer)
			=> _member.Root.With<CustomSerializationExtension>()
			          .Members.Apply(_member.GetMember(), serializer)
			          .Return(_member);

		public IMemberConfiguration<T, TMember> None() => _member.Root.With<CustomSerializationExtension>()
		                                                         .Members.Remove(_member.GetMember())
		                                                         .Return(_member);
	}
}