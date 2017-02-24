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

using System.Collections.Generic;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MemberContentsReader : DecoratedReader
	{
		readonly IDictionary<string, IMember> _members;

		public MemberContentsReader(IReader activator, IDictionary<string, IMember> members)
			: base(new MemberAttributesReader(activator, members))
		{
			_members = members;
		}

		public override object Get(IXmlReader parameter)
		{
			var result = base.Get(parameter);

			var token = parameter.Content.New();
			while (parameter.Content.Next(token))
			{
				var member = _members.Get(parameter.Name);
				member?.Adapter.Assign(result, member.Get(parameter));
			}

			return result;
		}
	}
}