using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Represents a source of member specifications.
	/// </summary>
	public interface IMemberSpecifications : IEnumerable<ISpecification<IMember>> {}
}