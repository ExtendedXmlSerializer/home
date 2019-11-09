using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Expressions
{
	interface IEvaluation : ISpecificationSource<TypeInfo, object>, ISource<Exception> {}
}