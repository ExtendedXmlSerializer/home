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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.ElementModel.Members
{
	sealed class RecursionGuardedElementMembers : IElementMembers
	{
		readonly ObjectIdGenerator _generator = new ObjectIdGenerator();

		readonly IElementMembers _members;

		public RecursionGuardedElementMembers(IElementMembers members)
		{
			_members = members;
		}

		public IMembers Get(TypeInfo parameter)
			=> _generator.For(parameter).FirstEncounter ? _members.Get(parameter) : new Deferred(_members.Build(parameter));

		sealed class Deferred : IMembers
		{
			readonly Func<IMembers> _members;

			public Deferred(Func<IMembers> members)
			{
				_members = members;
			}

			public IEnumerator<IMember> GetEnumerator() => _members().GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IMember Get(string parameter) => _members().Get(parameter);
		}
	}
}