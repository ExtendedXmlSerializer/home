using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class ClassicAllowedMemberValues : IAllowedMemberValues
	{
		readonly static IAllowedValueSpecification
			Always   = AlwaysEmitMemberSpecification.Default,
			Assigned = AllowAssignedValues.Default;

		public static ClassicAllowedMemberValues Default { get; } = new ClassicAllowedMemberValues();

		ClassicAllowedMemberValues() : this(IsValueTypeSpecification.Default) {}

		readonly ISpecification<TypeInfo> _valueType;

		public ClassicAllowedMemberValues(ISpecification<TypeInfo> valueType) => _valueType = valueType;

		public IAllowedValueSpecification Get(MemberInfo parameter) => From(parameter);

		IAllowedValueSpecification From(MemberDescriptor parameter)
			=> _valueType.IsSatisfiedBy(parameter.MemberType) ? Always : Assigned;
	}
}