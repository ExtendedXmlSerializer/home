using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface ITypeMemberDefaults : IParameterizedSource<TypeInfo, Func<MemberInfo, object>> {}
}