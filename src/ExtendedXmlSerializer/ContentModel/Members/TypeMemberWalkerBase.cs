using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	abstract class TypeMemberWalkerBase<T> : ObjectWalkerBase<TypeInfo, IEnumerable<T>>
	{
		readonly ITypeMembers _members;

		protected TypeMemberWalkerBase(ITypeMembers members, TypeInfo root) : base(root) => _members = members;

		protected override IEnumerable<T> Select(TypeInfo type) => Members(type);

		protected virtual IEnumerable<T> Members(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var length  = members.Length;

			for (var i = 0; i < length; i++)
			{
				var member = members[i];

				foreach (var item in Yield(member))
				{
					yield return item;
				}
			}
		}

		protected abstract IEnumerable<T> Yield(IMember member);
	}
}