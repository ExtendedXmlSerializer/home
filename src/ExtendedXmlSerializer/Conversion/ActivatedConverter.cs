using System.Reflection;
using ExtendedXmlSerialization.Conversion.Members;

namespace ExtendedXmlSerialization.Conversion
{
	class ActivatedConverter : ConverterBase
	{
		readonly IActivator _activator;
		readonly IMembers _members;

		public ActivatedConverter(IActivator activator, IMembers members, TypeInfo classification) : base(classification)
		{
			_activator = activator;
			_members = members;
		}

		public override void Emit(IWriter writer, object instance)
		{
			foreach (var member in _members)
			{
				var value = member.Get(instance);
				if (value != null)
				{
					member.Emit(writer, value);
				}
			}
		}

		public override object Get(IReader reader) => _activator.Get(reader);
	}
}