using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	interface IGenericActivation : IParameterizedSource<TypeInfo, Expression> {}
}