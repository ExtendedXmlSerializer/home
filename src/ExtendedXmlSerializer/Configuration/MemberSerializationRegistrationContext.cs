using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Establishes a serialization registration context for a member configuration.
	/// </summary>
	/// <typeparam name="T">The member's containing type.</typeparam>
	/// <typeparam name="TMember">The member's value type.</typeparam>
	public sealed class MemberSerializationRegistrationContext<T, TMember>
	{
		readonly IMemberConfiguration<T, TMember> _member;

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="member">The member configuration to configure.</param>
		public MemberSerializationRegistrationContext(IMemberConfiguration<T, TMember> member) => _member = member;

		/// <summary>
		/// Used to activate the specified strongly-typed definition and register it as this member's serializer.  Doing so
		/// will allow you to design your serializer to import dependencies into its constructor.
		/// </summary>
		/// <typeparam name="TSerializer">The serializer type to activate.</typeparam>
		/// <returns>The configured member configuration.</returns>
		public IMemberConfiguration<T, TMember> Of<TSerializer>() where TSerializer : ISerializer<TMember>
			=> Of(Support<TSerializer>.Key);

		/// <summary>
		/// Used to activate the specified type and register it as this member's serializer.  Doing so will allow you to
		/// design your serializer to import dependencies into its constructor.
		/// </summary>
		/// <returns>The configured member configuration.</returns>
		public IMemberConfiguration<T, TMember> Of(Type serializerType)
			=> Using(new ActivatedSerializer(serializerType, Support<TMember>.Metadata));

		/// <summary>
		/// Registers a new serializer with the provided delegates.
		/// </summary>
		/// <param name="serialize">The delegate to call when serializing an instance of the member's value.</param>
		/// <param name="deserialize">The delegate to call when deserializing an instance of the member's value.</param>
		/// <returns>The configured member configuration.</returns>
		public IMemberConfiguration<T, TMember> ByCalling(Action<IFormatWriter, TMember> serialize,
		                                                  Func<IFormatReader, TMember> deserialize)
			=> Using(new DelegatedSerializer<TMember>(serialize, deserialize));

		/// <summary>
		/// Provides an instance of a serializer to register as this member's serializer.
		/// </summary>
		/// <param name="serializer">The serializer to use to serialize/deserialize instances of this member's value.</param>
		/// <returns>The configured member configuration.</returns>
		public IMemberConfiguration<T, TMember> Using(ISerializer<TMember> serializer) => Using(serializer.Adapt());

		/// <summary>
		/// Provides an instance of a serializer to register as this member's serializer.
		/// </summary>
		/// <param name="serializer">The serializer to use to serialize/deserialize instances of this member's value.</param>
		/// <returns>The configured member configuration.</returns>
		public IMemberConfiguration<T, TMember> Using(ContentModel.ISerializer serializer)
			=> _member.Root.With<CustomSerializationExtension>()
			          .Members.Apply(_member.GetMember(), serializer)
			          .Return(_member);

		/// <summary>
		/// Clears any serializer that is registered with this type.  This will result in this member using the default
		/// serialization/deserialization mechanisms of the root serializer for this member's value type.
		/// </summary>
		/// <returns>The configured member configuration.</returns>
		public IMemberConfiguration<T, TMember> None() => _member.Root.With<CustomSerializationExtension>()
		                                                         .Members.Remove(_member.GetMember())
		                                                         .Return(_member);
	}
}