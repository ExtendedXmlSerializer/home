using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	sealed class MemberAccess : IMemberAccess
	{
		public ISpecification<object> Instance { get; }
		readonly ISpecification<object> _specification;
		readonly IProperty              _property;

		public MemberAccess(ISpecification<object> specification, IProperty property)
			: this(specification, specification.GetInstance(), property) {}

		public MemberAccess(ISpecification<object> specification, ISpecification<object> instance, IProperty property)
		{
			Instance       = instance;
			_specification = specification;
			_property      = property;
		}

		public bool IsSatisfiedBy(object parameter) => _specification.IsSatisfiedBy(parameter);

		public object Get(object instance) => _property.Get(instance);

		public void Assign(object instance, object value)
		{
			if (_specification.IsSatisfiedBy(value))
			{
				_property.Assign(instance, value);
			}
		}
	}
}