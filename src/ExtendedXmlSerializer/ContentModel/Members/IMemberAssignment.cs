using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IMemberAssignment
	{
		void Assign(IInnerContent contents, IMemberAccess access, object value);
	}
}