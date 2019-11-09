using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	interface ICoercer
		: ISpecification<object>,
		  ISpecification<TypeInfo>,
		  IParameterizedSource<CoercerParameter, object> {}

	//public interface ICoercer<in TFrom, out TTo> : IParameterizedSource<TFrom, TTo> {}
}