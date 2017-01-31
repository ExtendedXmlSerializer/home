using System;
using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class MemberConverter : NamedConverter<IMemberName>, IMemberConverter
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		/*public MemberContext(MemberInfo metadata, Action<object, object> setter, Func<object, object> getter,
		                     IElementContext context)
			: this(metadata.Name, metadata, setter, getter, context) {}*/

		public MemberConverter(IMemberName name, IConverter body, Action<object, object> setter,
		                     Func<object, object> getter) : this(name.DisplayName, name, body, setter, getter) {}

		public MemberConverter(string displayName, IMemberName name, IConverter body, Action<object, object> setter,
		                     Func<object, object> getter) : base(name, body)
		{
			DisplayName = displayName;
			_setter = setter;
			_getter = getter;
		}

		public string DisplayName { get; }

		public virtual object Get(object instance) => _getter(instance);

		public virtual void Assign(object instance, object value)
		{
			if (value != null)
			{
				_setter(instance, value);
			}
		}
	}
}