using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Serializers : ISerializers
	{
		readonly IElement    _element;
		readonly IContents   _contents;
		readonly IEnclosures _enclosures;

		public Serializers(IElement element, IContents contents, IEnclosures enclosures)
		{
			_element    = element;
			_contents   = contents;
            _enclosures = enclosures;

        }

		public ISerializer Get(TypeInfo parameter)
		{
			var element = _element.Get(parameter);
			var result  = new Container(element, _contents.Get(parameter), _enclosures);
			return result;
		}
	}
}