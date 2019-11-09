using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicIdentificationExtension : ISerializerExtension
	{
		public ClassicIdentificationExtension() : this(new Collection<TypeInfo>()) {}

		public ClassicIdentificationExtension(ICollection<TypeInfo> types) => Types = types;

		public ICollection<TypeInfo> Types { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<ITypeIdentity>(TypeIdentity.Default)
			            .Register<ITypeIdentityRegistrations, TypeIdentityRegistrations>()
			            .Register<ITypeIdentifications, TypeIdentifications>()
			            .Decorate<IIdentifiers, Identifiers>()
			            .Decorate<ITypeIdentities, TypeIdentities>()
			            .Register(Register);

		ITypeIdentification Register(IServiceProvider services) => services.Get<ITypeIdentifications>()
		                                                                   .Get(Types);

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}