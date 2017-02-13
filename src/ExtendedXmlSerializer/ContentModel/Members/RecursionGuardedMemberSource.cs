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

namespace ExtendedXmlSerialization.ContentModel.Members
{
	sealed class RecursionGuardedMemberSource : IMemberSource
	{
		readonly ObjectIdGenerator _generator = new ObjectIdGenerator();

		readonly IMemberSource _members;

		public RecursionGuardedMemberSource(IMemberSource members)
		{
			_members = members;
		}

		public IEnumerable<IMember> Get(TypeInfo parameter)
			=> _generator.For(parameter).FirstEncounter ? _members.Get(parameter) : new Deferred(_members.Build(parameter));

		sealed class Deferred : IEnumerable<IMember>
		{
			readonly Func<IEnumerable<IMember>> _members;

			public Deferred(Func<IEnumerable<IMember>> members)
			{
				_members = members;
			}

			public IEnumerator<IMember> GetEnumerator() => _members().GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}