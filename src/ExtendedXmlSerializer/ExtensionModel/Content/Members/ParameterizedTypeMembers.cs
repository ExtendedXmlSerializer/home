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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class ParameterizedTypeMembers : ITypeMembers
	{
		readonly ITypeMembers _typed;
		readonly IParameterizedMembers _members;

		public ParameterizedTypeMembers(ITypeMembers typed, IParameterizedMembers members)
		{
			_typed = typed;
			_members = members;
		}

		public ImmutableArray<IMember> Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var typed = _typed.Get(parameter);
			var items = members?.AddRange(typed) ?? typed;

			var result = items.GroupBy(IdentityFormatter.Default.Get)
			                  .Select(x => x.OfType<ParameterizedMember>().FirstOrDefault() ?? x.First())
			                  .ToImmutableArray();
			return result;
		}
	}
}