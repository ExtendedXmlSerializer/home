using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	abstract class CoercerBase<TFrom, TTo> : ICoercer
	{
		readonly static ISpecification<object>   From = IsInstanceOfTypeSpecification<TFrom>.Default;
		readonly static ISpecification<TypeInfo> To   = IsAssignableSpecification<TTo>.Default;

		readonly ISpecification<object>   _from;
		readonly ISpecification<TypeInfo> _to;

		protected CoercerBase() : this(To) {}

		protected CoercerBase(ISpecification<TypeInfo> to) : this(From, to) {}

		protected CoercerBase(ISpecification<object> from, ISpecification<TypeInfo> to)
		{
			_from = from;
			_to   = to;
		}

		public bool IsSatisfiedBy(object parameter) => _from.IsSatisfiedBy(parameter);

		public bool IsSatisfiedBy(TypeInfo parameter) => _to.IsSatisfiedBy(parameter);

		protected abstract TTo Get(TFrom parameter, TypeInfo targetType);

		object IParameterizedSource<CoercerParameter, object>.Get(CoercerParameter parameter)
			=> Get((TFrom)parameter.Instance, parameter.TargetType);
	}
}