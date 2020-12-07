using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IInstanceMemberSerialization : IParameterizedSource<IInnerContent, IMemberSerialization>, 
	                                         IParameterizedSource<object, IMemberSerialization> {}
}