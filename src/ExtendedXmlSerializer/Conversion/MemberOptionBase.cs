using ExtendedXmlSerialization.Conversion.Model;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion
{
	public abstract class MemberOptionBase : OptionBase<MemberInformation, IMemberContext>, IMemberOption
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

		public override IMemberContext Get(MemberInformation parameter) => Create(_provider.Get(parameter));

		protected abstract IMemberContext Create(IMemberName name);
	}
}