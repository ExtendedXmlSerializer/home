using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class InnerContentReader : IReader
	{
		readonly IInnerContentActivator _activator;
		readonly IInnerContentHandler   _content;
		readonly IInnerContentResult    _result;

		public InnerContentReader(IInnerContentActivator activator, IInnerContentHandler content,
		                          IInnerContentResult result)
		{
			_activator = activator;
			_content   = content;
			_result    = result;
		}

		public object Get(IFormatReader parameter)
		{
			var content = _activator.Get(parameter);
			if (content != null)
			{
				while (content.MoveNext())
				{
					_content.IsSatisfiedBy(content);
				}

				var result = _result.Get(content);
				return result;
			}

			return null;
		}
	}
}