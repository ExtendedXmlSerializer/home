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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Content;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MemberSource : IMemberSource
	{
		readonly ISerialization _serialization;
		readonly Func<IMemberProfile, IMember> _select;

		public MemberSource(ISerialization serialization, ISelector selector) : this(serialization, selector.Get) {}

		public MemberSource(ISerialization serialization, Func<IMemberProfile, IMember> select)
		{
			_serialization = serialization;
			_select = select;
		}

		public IEnumerable<IMember> Get(TypeInfo parameter)
			=> Yield(parameter).OrderBy(x => x.Order).Select(_select).Where(x => x != null);

		IEnumerable<IMemberProfile> Yield(TypeInfo parameter)
		{
			var properties = parameter.GetProperties();
			var length = properties.Length;
			for (var i = 0; i < length; i++)
			{
				var profile = _serialization.Get(new Property(properties[i]));
				if (profile != null)
				{
					yield return profile;
				}
			}

			var fields = parameter.GetFields();
			var l = fields.Length;
			for (var i = 0; i < l; i++)
			{
				var profile = _serialization.Get(new Field(fields[i]));
				if (profile != null)
				{
					yield return profile;
				}
			}
		}
	}
}