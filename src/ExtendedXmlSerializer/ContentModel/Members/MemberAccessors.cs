using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberAccessors : DecoratedSource<IMember, IMemberAccess>, IMemberAccessors
	{
		public MemberAccessors(WritableMemberAccessors accessors, ReadOnlyCollectionAccessors @readonly)
			: this((IParameterizedSource<IMember, IMemberAccess>)accessors, @readonly) {}

		MemberAccessors(IParameterizedSource<IMember, IMemberAccess> accessors,
		                IParameterizedSource<IMember, IMemberAccess> @readonly)
			: base(@readonly.If(ReadOnlyCollectionSpecification.Default)
			                .Let(new DelegatedSpecification<IMember>(x => x.IsWritable),
			                     AlteredValueSpecification<IMemberAccess>.Default, accessors)) {}
	}
}