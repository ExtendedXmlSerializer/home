using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMemberConverter>, IMemberOption
	{
		readonly IParameterizedSource<MemberInformation, IMemberName> _provider;

		protected MemberOptionBase(ISpecification<MemberInformation> specification)
			: this(specification, MemberNameProvider.Default) {}

		protected MemberOptionBase(ISpecification<MemberInformation> specification,
		                           IParameterizedSource<MemberInformation, IMemberName> provider)
			: base(specification)
		{
			_provider = provider;
		}

		public override IMemberConverter Get(MemberInformation parameter) => Create(_provider.Get(parameter));

		protected abstract IMemberConverter Create(IMemberName name);
	}
}