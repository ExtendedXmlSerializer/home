using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IActivators : IParameterizedSource<Type, IActivator> {}
}