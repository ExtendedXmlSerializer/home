using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ReflectionModel
{
	public interface IGenericActivation : IParameterizedSource<TypeInfo, Expression> {}
}