using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberInnerContentHandler : IInnerContentHandler
	{
		readonly IInstanceMemberSerialization _serialization;
		readonly IMemberHandler               _handler;
		readonly IReaderFormatter             _formatter;

		public MemberInnerContentHandler(IInstanceMemberSerialization serialization, IMemberHandler handler,
		                                 IReaderFormatter formatter)
		{
			_serialization = serialization;
			_handler       = handler;
			_formatter     = formatter;
		}

		public bool IsSatisfiedBy(IInnerContent parameter)
		{
			var content       = parameter.Get();
			var key           = _formatter.Get(content);
			var serialization = _serialization.Get(parameter);
			if (serialization != null)
			{
				var member = serialization.Get(key);
				var result = member != null;
				if (result)
				{
					_handler.Handle(parameter, member);
					return true;
				}
			}

			return false;
		}
	}
}