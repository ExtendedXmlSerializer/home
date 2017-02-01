using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberSelector : OptionSelector<MemberInformation, IMember>, IMemberSelector
	{
		public MemberSelector(IConverters converters, IAddDelegates add)
			: base(new MemberOption(converters), new ReadOnlyCollectionMemberOption(converters, add)) {}
	}
}