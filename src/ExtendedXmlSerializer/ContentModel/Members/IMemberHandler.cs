using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IMemberHandler
	{
		void Handle(IInnerContent contents, IMemberSerializer member);
	}
}