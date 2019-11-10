using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class AttachedPropertiesExtension : ISerializerExtension
	{
		[UsedImplicitly]
		public AttachedPropertiesExtension() : this(new Registrations<IProperty>()) {}

		public AttachedPropertiesExtension(Registrations<IProperty> registrations)
		{
			Registrations = registrations;
		}

		public Registrations<IProperty> Registrations { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> Registrations.Alter(parameter)
			                .Register(x => x.GetAllInstances<IProperty>()
			                                .ToImmutableArray())
			                .Decorate<IMemberAccessors, MemberAccessors>()
			                .Decorate<ITypeMembers, TypeMembers>()
			                .Decorate<IReaderFormatter, ReaderFormatter>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}