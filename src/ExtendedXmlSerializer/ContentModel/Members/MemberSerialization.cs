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
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberSerialization : TableSource<string, IMemberSerializer>, IMemberSerialization
	{
		readonly IRuntimeMemberList _runtime;
		readonly ImmutableArray<IMemberSerializer> _all;

		public MemberSerialization(IRuntimeMemberList runtime, ImmutableArray<IMemberSerializer> all)
			: base(all.ToDictionary(x => IdentityFormatter.Default.Get(x.Profile)))
		{
			_runtime = runtime;
			_all = all;
		}

		public ImmutableArray<IMemberSerializer> Get(object parameter) => _runtime.Get(parameter);

		public ImmutableArray<IMemberSerializer> Get() => _all;
	}




	sealed class MemberAssignment<T, TValue> : IMemberAssignment<T>
	{
		readonly IParameterizedSource<string, MemberProfile<T, TValue>?> _members;
		readonly IReaderFormatter _formatter;

		/*new TableSource<string, MemberProfile<T>?>(all.ToDictionary(x => IdentityFormatter.Default.Get(x.Member), x => new MemberProfile<T>?(x))), formatter*/

		public MemberAssignment(IParameterizedSource<string, MemberProfile<T, TValue>?> members, IReaderFormatter formatter)
		{
			_members = members;
			_formatter = formatter;
		}

		public void Execute(InnerContent<T> parameter)
		{
			var key = _formatter.Get(parameter.Reader);
			var member = _members.Get(key);
			member?.Access.Assign(parameter.Content.Current, member.Value.Serializer.Get(parameter.Reader));
		}
	}

/*
	sealed class MemberSerialization<T> : IMemberSerialization<T>
	{
		readonly IRuntimeMemberList<T> _runtime;
		readonly ImmutableArray<MemberProfile<T>> _all;

		public MemberSerialization(IRuntimeMemberList<T> runtime, ImmutableArray<MemberProfile<T>> all)
		{
			_runtime = runtime;
			_all = all;
		}

		ImmutableArray<MemberProfile<T>> ISource<ImmutableArray<MemberProfile<T>>>.Get() => _all;

		public ImmutableArray<MemberProfile<T>> Get(T parameter) => _runtime.Get(parameter);
	}
*/

}