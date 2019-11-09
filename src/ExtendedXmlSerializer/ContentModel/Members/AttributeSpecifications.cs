using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class AttributeSpecifications
		: TableSource<MemberInfo, IAttributeSpecification>,
		  IAttributeSpecifications
	{
		public AttributeSpecifications(IDictionary<MemberInfo, IAttributeSpecification> store) : base(store) {}
	}
}