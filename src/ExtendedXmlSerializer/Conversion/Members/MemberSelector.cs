using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberSelector : OptionSelector<MemberInformation, IMember>, IMemberSelector
	{
		public MemberSelector(IConverters converters, IAddDelegates add, IMemberAdorner adorner)
			: base(new MemberOption(converters, adorner), new ReadOnlyCollectionMemberOption(converters, add)) {}
	}
}