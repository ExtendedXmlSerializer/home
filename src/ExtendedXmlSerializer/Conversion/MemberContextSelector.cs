using ExtendedXmlSerialization.Conversion.Model;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	class MemberContextSelector : OptionSelector<MemberInformation, IMemberContext>, IMemberContextSelector
	{
		public MemberContextSelector(IContexts contexts, IAddDelegates add)
			: this(new MemberOption(contexts), new ReadOnlyCollectionMemberOption(contexts, add)) {}

		public MemberContextSelector(params IOption<MemberInformation, IMemberContext>[] options) : base(options) {}
	}
}