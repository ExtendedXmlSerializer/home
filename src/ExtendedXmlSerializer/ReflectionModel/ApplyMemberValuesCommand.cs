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

using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ReflectionModel
{
	sealed class ApplyMemberValuesCommand : ICommand<object>
	{
		readonly IMemberAccessors _accessors;
		readonly ImmutableArray<IMember> _members;
		readonly ITableSource<string, object> _values;

		public ApplyMemberValuesCommand(IMemberAccessors accessors, ImmutableArray<IMember> members,
		                                ITableSource<string, object> values)
		{
			_accessors = accessors;
			_members = members;
			_values = values;
		}

		public void Execute(object parameter)
		{
			foreach (var member in _members)
			{
				if (_values.IsSatisfiedBy(member.Name))
				{
					var access = _accessors.Get(member);
					var value = _values.Get(member.Name);
					access.Assign(parameter, value);
				}
			}
		}
	}
}