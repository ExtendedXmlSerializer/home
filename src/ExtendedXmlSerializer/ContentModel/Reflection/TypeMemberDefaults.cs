using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	sealed class TypeMemberDefaults : ReferenceCacheBase<TypeInfo, Func<MemberInfo, object>>, ITypeMemberDefaults
	{
		/*public static TypeMemberDefaults Default { get; } = new TypeMemberDefaults();

		TypeMemberDefaults() : this(TypeDefaults.Default) {}*/

		readonly ITypeDefaults _defaults;

		public TypeMemberDefaults(ITypeDefaults defaults) => _defaults = defaults;

		protected override Func<MemberInfo, object> Create(TypeInfo parameter)
			=> new MemberDefaults(_defaults.Get(parameter)).Get;
	}
}