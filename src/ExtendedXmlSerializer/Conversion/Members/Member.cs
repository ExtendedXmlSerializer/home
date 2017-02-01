using System;
using System.Reflection;
using ExtendedXmlSerialization.Conversion.Names;

namespace ExtendedXmlSerialization.Conversion.Members
{
	public class Member : ContainerBase, IMember
	{
		readonly Action<object, object> _setter;
		readonly Func<object, object> _getter;

		public Member(string displayName, TypeInfo classification, Action<object, object> setter, Func<object, object> getter, IConverter body) : base(body)
		{
			DisplayName = displayName;
			Classification = classification;
			_setter = setter;
			_getter = getter;
		}

		public string DisplayName { get; }
		public TypeInfo Classification { get; }

		public virtual object Get(object instance) => _getter(instance);

		public virtual void Assign(object instance, object value)
		{
			if (value != null)
			{
				_setter(instance, value);
			}
		}

		protected override IName Name => this;
	}
}