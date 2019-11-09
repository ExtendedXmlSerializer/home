using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Conversion;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberConverters : IMemberConverters
	{
		readonly IMemberConverterSpecification _specification;
		readonly IConverters                   _converters;

		public MemberConverters(IMemberConverterSpecification specification, IConverters converters)
		{
			_specification = specification;
			_converters    = converters;
		}

		public IConverter Get(MemberInfo parameter) => _specification.IsSatisfiedBy(parameter) ? From(parameter) : null;

		IConverter From(MemberDescriptor descriptor)
		{
			var result = _converters.Get(descriptor.MemberType);
			if (result != null)
			{
				return result;
			}

			throw new InvalidOperationException(
			                                    $"An attempt was made to format '{descriptor.Metadata}' as an attribute, but there is not a registered converter that can convert its values to a string.  Please ensure a converter is registered for the type '{descriptor.MemberType}' by adding a converter for this type to the converter collection in the ConverterExtension.");
		}
	}
}