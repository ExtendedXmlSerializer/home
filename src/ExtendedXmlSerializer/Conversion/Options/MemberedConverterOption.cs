using System.Reflection;
using ExtendedXmlSerialization.Conversion.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion.Options
{
	class MemberedConverterOption : ConverterOptionBase
	{
		readonly IActivators _activators;
		readonly ITypeMembers _members;

		public MemberedConverterOption(ITypeMembers members) : this(Activators.Default, members) {}

		public MemberedConverterOption(IActivators activators, ITypeMembers members)
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
			var result = new DecoratedConverter(activator, new MemberEmitter(members));
			return result;
		}
	}
}