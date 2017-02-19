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
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ContentModel.Members
{
	class MemberSource : IMemberSource
	{
		readonly static Func<MemberInformation, int> Order = MemberOrder.Default.Get;

		readonly Func<MemberInformation, int> _order;
		readonly ISerialization _serialization;
		readonly Func<MemberInformation, IMember> _select;

		public MemberSource(ISerialization serialization, ISelector selector) : this(Order, serialization, selector.Get) {}

		public MemberSource(Func<MemberInformation, int> order, ISerialization serialization,
		                    Func<MemberInformation, IMember> select)
		{
			_order = order;
			_serialization = serialization;
			_select = select;
		}

		public IEnumerable<IMember> Get(TypeInfo parameter)
			=> Yield(parameter).OrderBy(_order).Select(_select).Where(x => x != null);

		IEnumerable<MemberInformation> Yield(TypeInfo parameter)
		{
			var properties = parameter.GetProperties();
			var length = properties.Length;
			for (var i = 0; i < length; i++)
			{
				var property = properties[i];
				if (_serialization.IsSatisfiedBy(property))
				{
					yield return Create(property, property.PropertyType, property.CanWrite);
				}
			}

			var fields = parameter.GetFields();
			var l = fields.Length;
			for (var i = 0; i < l; i++)
			{
				var field = fields[i];
				if (_serialization.IsSatisfiedBy(field))
				{
					yield return Create(field, field.FieldType, !field.IsInitOnly);
				}
			}
		}

		static MemberInformation Create(MemberInfo metadata, Type memberType, bool assignable)
			=> new MemberInformation(metadata, memberType.GetTypeInfo().AccountForNullable(), assignable);
	}
}