using System;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	interface IVariableTypeSpecification : ISpecification<TypeInfo>, ISpecification<Type> {}
}