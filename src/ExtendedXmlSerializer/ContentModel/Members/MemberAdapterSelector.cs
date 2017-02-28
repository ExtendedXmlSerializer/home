using System;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MemberAdapterSelector : IParameterizedSource<MemberProfile, IMemberAdapter>
	{
		readonly Func<object, object> _getter;
		readonly Action<object, object> _setter;

		public MemberAdapterSelector(Func<object, object> getter, Action<object, object> setter)
		{
			_getter = getter;
			_setter = setter;
		}

		public IMemberAdapter Get(MemberProfile parameter)
			=> new MemberAdapter(parameter.Specification, parameter.Identity.Name, parameter.Metadata,
			                     parameter.MemberType, parameter.AllowWrite, _getter, _setter);
	}
}