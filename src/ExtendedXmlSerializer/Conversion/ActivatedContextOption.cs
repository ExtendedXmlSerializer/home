using System.Reflection;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Conversion
{
	class ActivatedContextOption : ContextOptionBase
	{
		readonly IActivators _activators;
		readonly IContextMembers _members;

		public ActivatedContextOption(IActivators activators, IContextMembers members)
			: base(IsActivatedTypeSpecification.Default)
		{
			_activators = activators;
			_members = members;
		}

		public override IElementContext Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var activate = _activators.Get(parameter.AsType());
			var activator = new MemberedActivator(new DelegatedActivator(activate), members);
			var result = new ActivatedContext(activator, members, parameter);
			return result;
		}
	}
}