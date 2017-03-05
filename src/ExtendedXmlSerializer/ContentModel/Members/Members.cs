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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class Members : IMembers
	{
		readonly IMemberSerialization _serialization;
		readonly Func<MemberProfile, IMember> _select;

		[UsedImplicitly]
		public Members(IMemberSerialization serialization, ISelector selector) : this(serialization, selector.Get) {}

		public Members(IMemberSerialization serialization, Func<MemberProfile, IMember> select)
		{
			_serialization = serialization;
			_select = select;
		}

		public ImmutableArray<IMember> Get(TypeInfo parameter) =>
			Yield(parameter).OrderBy(x => x.Writer is IPropertySerializer ? 0 : 1)
			                .ThenBy(x => x.Order)
			                .Select(_select)
			                .Where(x => x != null)
			                .ToImmutableArray();

		IEnumerable<MemberProfile> Yield(TypeInfo parameter)
		{
			var properties = parameter.GetProperties();
			var length = properties.Length;
			for (var i = 0; i < length; i++)
			{
				var property = properties[i];
				if (_serialization.IsSatisfiedBy(property))
				{
					yield return
						_serialization.Get(new MemberDescriptor(parameter, property));
				}
			}

			var fields = parameter.GetFields();
			var l = fields.Length;
			for (var i = 0; i < l; i++)
			{
				var field = fields[i];
				if (_serialization.IsSatisfiedBy(field))
				{
					yield return
						_serialization.Get(new MemberDescriptor(parameter, field));
				}
			}
		}
	}
}