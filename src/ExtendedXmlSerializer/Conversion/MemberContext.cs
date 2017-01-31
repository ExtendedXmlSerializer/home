using System;

namespace ExtendedXmlSerialization.Conversion
{
	public class MemberContext : NamedContext<IMemberName>, IMemberContext
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		/*public MemberContext(MemberInfo metadata, Action<object, object> setter, Func<object, object> getter,
		                     IElementContext context)
			: this(metadata.Name, metadata, setter, getter, context) {}*/

		public MemberContext(IMemberName name, IElementContext body, Action<object, object> setter,
		                     Func<object, object> getter) : this(name.DisplayName, name, body, setter, getter) {}

		public MemberContext(string displayName, IMemberName name, IElementContext body, Action<object, object> setter,
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