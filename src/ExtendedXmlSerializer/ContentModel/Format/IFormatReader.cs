using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	public interface IFormatReader : IFormat, ISpecification<IIdentity>, IIdentity, IReflectionParser
	{
		string Content();

		void Set();
	}
}