using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IGetterFactory : IParameterizedSource<MemberInfo, Func<object, object>> {}
}