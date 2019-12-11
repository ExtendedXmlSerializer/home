using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class Serializers : ISerializers
	{
		readonly IElement  _element;
		readonly IContents _contents;

		public Serializers(IElement element, IContents contents)
		{
			_element  = element;
			_contents = contents;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var element = _element.Get(parameter);
			var result  = new Container(element, _contents.Get(parameter));
			return result;
		}
	}
}