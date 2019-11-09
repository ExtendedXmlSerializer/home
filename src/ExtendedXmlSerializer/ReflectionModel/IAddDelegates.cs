using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IAddDelegates : IParameterizedSource<TypeInfo, Action<object, object>> {}
}