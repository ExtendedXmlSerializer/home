// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
			var content = parameter.Get();
			var key     = _formatter.Get(content);
			var member = _serialization.Get(parameter.Current)
			                           .Get(key);
			var result = member != null;
			if (result)
			{
				_handler.Handle(parameter, member);	
			}

			return result;
		}
	}
}