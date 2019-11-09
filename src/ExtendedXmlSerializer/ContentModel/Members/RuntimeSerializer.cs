using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class RuntimeSerializer : IRuntimeSerializer
	{
		readonly ISpecification<object> _specification;
		readonly IMemberSerializer      _property;
		readonly IMemberSerializer      _content;

		public RuntimeSerializer(ISpecification<object> specification, IMemberSerializer property,
		                         IMemberSerializer content)
		{
			_specification = specification;
			_property      = property;
			_content       = content;
		}

		public IMemberSerializer Get(object parameter)
			=> _specification.IsSatisfiedBy(parameter) ? _property : _content;

		public object Get(IFormatReader parameter) => _content.Get(parameter);

		public void Write(IFormatWriter writer, object instance) => _content.Write(writer, instance);

		public IMember Profile => _content.Profile;

		public IMemberAccess Access => _content.Access;
	}
}