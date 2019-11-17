using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	/// <summary>
	/// Root-level reader object used during deserialization.
	/// </summary>
	public interface IFormatReader : IFormat, ISpecification<IIdentity>, IIdentity, IReflectionParser
	{
		/// <summary>
		/// Gets the content found within the current xml document element being processed.
		/// </summary>
		/// <returns>Text (if any) representing the current content.</returns>
		string Content();

		/// <summary>
		/// Used to set the current reader so that it is located on content and not within attribute data.
		/// </summary>
		void Set();
	}
}