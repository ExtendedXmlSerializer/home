using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class AttributeSpecifications : IAttributeSpecifications
	{
		readonly static TypeInfo               Type   = typeof(string).GetTypeInfo();
		readonly static AttributeSpecification Always = new AttributeSpecification(AlwaysSpecification<object>.Default);

		readonly ISpecification<TypeInfo> _source;
		readonly IAttributeSpecification  _text;
		readonly IAttributeSpecifications _specifications;

		public AttributeSpecifications(ISpecification<TypeInfo> source, IAttributeSpecification text,
		                               IAttributeSpecifications specifications)
		{
			_source         = source;
			_text           = text;
			_specifications = specifications;
		}

		public IAttributeSpecification Get(MemberInfo parameter) => _specifications.Get(parameter) ?? From(parameter);

		IAttributeSpecification From(MemberDescriptor descriptor)
		{
			var supported = _source.IsSatisfiedBy(descriptor.MemberType);
			var result    = supported ? Equals(descriptor.MemberType, Type) ? _text : Always : null;
			return result;
		}
	}
}