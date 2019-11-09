using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public interface IMemberSpecifications : IEnumerable<ISpecification<IMember>> {}
}