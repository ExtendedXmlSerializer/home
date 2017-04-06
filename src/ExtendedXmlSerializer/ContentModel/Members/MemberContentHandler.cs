// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberContentHandler : IContentHandler, ISpecification<IContentsAdapter>
	{
		readonly IMemberSerialization _serialization;
		readonly IMemberAssignment _assignment;
		readonly IContentAdapterFormatter _formatter;

		public MemberContentHandler(IMemberSerialization serialization, IMemberAssignment assignment,
		                            IContentAdapterFormatter formatter)
		{
			_serialization = serialization;
			_assignment = assignment;
			_formatter = formatter;
		}

		public bool IsSatisfiedBy(IContentsAdapter parameter)
		{
			var content = parameter.Get();
			var member = _serialization.Get(_formatter.Get(content));
			var result = member != null;
			if (result)
			{
				_assignment.Assign(member, parameter, member.Access);
			}
			return result;
		}

		public void Execute(IContentsAdapter parameter) => IsSatisfiedBy(parameter);
	}
}