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

using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	interface IMemberSerializations : IParameterizedSource<TypeInfo, IMemberSerialization> {}

	interface IMemberContentWriters<T> : IParameterizedSource<IMember, IMemberContentWriter<T>> {}

	sealed class MemberContentWriters<T> : IMemberContentWriters<T>
	{
		readonly IMemberWriters<T> _contents;

		public MemberContentWriters(IMemberWriters<T> writers) => _contents = writers;

		public IMemberContentWriter<T> Get(IMember parameter) => new MemberContentWriter<T>(_contents.Get(parameter), parameter);
	}

	// ReSharper disable once PossibleInfiniteInheritance
	interface IContentWriters<T> : ISource<ImmutableArray<IMemberContentWriter<T>>> { }

	// ReSharper disable once PossibleInfiniteInheritance
	sealed class ContentWriters<T> : IContentWriters<T>
	{
		readonly Func<ImmutableArray<IMember>> _members;
		readonly Func<IMember, IMemberContentWriter<T>> _contents;
		readonly IMemberContentsAlteration<T> _alteration;

		public ContentWriters(IContentMembers members, IMemberContentWriters<T> writers, IMemberContentsAlteration<T> alteration)
			: this(members.ToDelegate(Support<T>.Key), writers.Get, alteration) {}

		public ContentWriters(Func<ImmutableArray<IMember>> members, Func<IMember, IMemberContentWriter<T>> contents, IMemberContentsAlteration<T> alteration)
		{
			_members = members;
			_contents = contents;
			_alteration = alteration;
		}


		public ImmutableArray<IMemberContentWriter<T>> Get() => _alteration.Get(_members.Invoke()
		                                                                                .Select(_contents))
		                                                                   .ToImmutableArray();
	}
}