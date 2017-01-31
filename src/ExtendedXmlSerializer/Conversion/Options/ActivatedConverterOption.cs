using System.Reflection;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Options
{
	class ActivatedConverterOption : ConverterOptionBase
	{
		readonly IActivators _activators;
		readonly IConverterMembers _members;

		public ActivatedConverterOption(IActivators activators, IConverterMembers members)
			: base(IsActivatedTypeSpecification.Default)
		{
			_activators = activators;
			_members = members;
		}

		public override IConverter Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var activate = _activators.Get(parameter.AsType());
			var activator = new MemberedActivator(new DelegatedActivator(activate), members);
			var result = new ActivatedConverter(activator, members, parameter);
			return result;
		}
	}
}