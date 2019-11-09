using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface ISetterFactory : IParameterizedSource<MemberInfo, Action<object, object>> {}
}