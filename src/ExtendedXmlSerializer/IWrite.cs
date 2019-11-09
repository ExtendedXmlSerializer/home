using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer
{
	interface IWrite<T> : ICommand<Writing<T>> {}
}