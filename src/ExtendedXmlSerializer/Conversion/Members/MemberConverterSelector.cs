using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Members
{
	class MemberConverterSelector : OptionSelector<MemberInformation, IMemberConverter>, IMemberConverterSelector
	{
		public MemberConverterSelector(IConverters converters, IAddDelegates add)
			: this(new MemberOption(converters), new ReadOnlyCollectionMemberOption(converters, add)) {}

		public MemberConverterSelector(params IOption<MemberInformation, IMemberConverter>[] options) : base(options) {}
	}
}