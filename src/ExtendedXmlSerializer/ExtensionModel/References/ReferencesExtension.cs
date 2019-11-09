using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferencesExtension : TypedTable<MemberInfo>, IEntityMembers, ISerializerExtension
	{
		public ReferencesExtension() : this(new Dictionary<TypeInfo, MemberInfo>()) {}

		public ReferencesExtension(IDictionary<TypeInfo, MemberInfo> store) : base(store) {}

		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter.Register<IRootReferences, RootReferences>()
			         .RegisterInstance<IEntityMembers>(this)
			         .RegisterInstance<IReferenceMaps>(ReferenceMaps.Default)
			         .Register<IReferenceEncounters, ReferenceEncounters>()
			         .Register<IEntities, Entities>()
			         .Decorate<IActivation, ReferenceActivation>()
			         .Decorate<ISerializers, CircularReferenceEnabledSerialization>()
			         .Decorate<IContents, ReferenceContents>()
			         .Decorate<IContents, RecursionAwareContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}