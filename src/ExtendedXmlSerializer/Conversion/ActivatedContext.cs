using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	class ActivatedContext : ContextBase
	{
		readonly IActivator _activator;
		readonly IMembers _members;

		public ActivatedContext(IActivator activator, IMembers members, TypeInfo classification) : base(classification)
		{
			_activator = activator;
			_members = members;
		}

		public override void Emit(IEmitter emitter, object instance)
		{
			foreach (var member in _members)
			{
				var value = member.Get(instance);
				if (value != null)
				{
					member.Emit(emitter, value);
				}
			}
		}

		public override object Yield(IYielder yielder) => _activator.Get(yielder);
	}
}