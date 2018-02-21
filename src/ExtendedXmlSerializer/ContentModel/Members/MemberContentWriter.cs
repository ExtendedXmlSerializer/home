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

using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberListWriter : IWriter
	{
		readonly IMemberSerialization _members;

		public MemberListWriter(IMemberSerialization members) => _members = members;

		public void Write(IFormatWriter writer, object instance)
		{
			if (instance != null)
			{
				var members = _members.Get(instance);
				var length = members.Length;
				for (var i = 0; i < length; i++)
				{
					members[i].Write(writer, instance);
				}
			}
			else
			{
				writer.Content(null);
			}
		}
	}

	interface IRuntimeContentComposer<T> : IParameterizedSource<IRuntimePipelinePart<T>, IRuntimeContent<T>> { }

	// ReSharper disable PossibleInfiniteInheritance
	interface IRuntimeContents<T> : ISource<ImmutableArray<IRuntimeContent<T>>> {}

	sealed class RuntimeContents<T> : FixedInstanceSource<ImmutableArray<IRuntimeContent<T>>>, IRuntimeContents<T>
	{
		public RuntimeContents(IContentWriters<T> writers, IRuntimeMemberContentSelectors<T> selectors)
			: base(writers.Get()
			              .Select(selectors.Get)
			              .ToImmutableArray()) {}
	}

	// ReSharper disable once PossibleInfiniteInheritance
	interface IRuntimeMemberContentWriters<T> : IParameterizedSource<Writing<T>, ImmutableArray<IContentWriter<T>>> { }

	interface IMemberContentsAlteration<T> : IEnumerableAlteration<IMemberContentWriter<T>> {}

	sealed class MemberContentsAlteration<T> : OrderByAlteration<IMemberContentWriter<T>, int>, IMemberContentsAlteration<T>
	{
		public static MemberContentsAlteration<T> Default { get; } = new MemberContentsAlteration<T>();
		MemberContentsAlteration() : base(x => x.Get().Order) {}
	}



	sealed class RuntimeMemberContentWriters<T> : IRuntimeMemberContentWriters<T>
	{
		readonly ImmutableArray<IRuntimeContent<T>> _selectors;
		readonly IMemberContentsAlteration<T> _alteration;
		readonly int _length;

		public RuntimeMemberContentWriters(IRuntimeContents<T> selectors, IMemberContentsAlteration<T> alteration)
			: this(selectors.Get(), alteration) {}

		public RuntimeMemberContentWriters(ImmutableArray<IRuntimeContent<T>> selectors, IMemberContentsAlteration<T> alteration)
			: this(selectors, alteration, selectors.Length) {}

		public RuntimeMemberContentWriters(ImmutableArray<IRuntimeContent<T>> selectors, IMemberContentsAlteration<T> alteration, int length)
		{
			_selectors = selectors;
			_alteration = alteration;
			_length = length;
		}

		public ImmutableArray<IContentWriter<T>> Get(Writing<T> parameter)
		{
			var contents = new IRuntimeContentWriter<T>[_length];
			for (var i = 0; i < _length; i++)
			{
				contents[i] = _selectors[i].Get(parameter);
			}

			var result = _alteration.Get(contents)
			                        .ToImmutableArray<IContentWriter<T>>();
			return result;
		}
	}

	interface IRuntimeMemberContentSelectors<T> : IParameterizedSource<IMemberContentWriter<T>, IRuntimeContent<T>> {}

	sealed class RuntimeMemberContentSelectors<T> : IRuntimeMemberContentSelectors<T>
	{
		readonly IRuntimeContentComposer<T> _composer;

		public RuntimeMemberContentSelectors(IRuntimeContentComposer<T> composer) => _composer = composer;

		public IRuntimeContent<T> Get(IMemberContentWriter<T> parameter)
		{
			var member = parameter.Get();
			var writer = new RuntimeContentWriter<T>(parameter, member);
			var content = new FixedRuntimeContent<T>(writer);
			var part = new RuntimePipelinePart<T>(writer, content);
			var result = _composer.Get(part);
			return result;
		}
	}

	interface IRuntimePipelinePart<T> : IRuntimeContentWriter<T>, IRuntimeContent<T> { }

	class RuntimePipelinePart<T> : IRuntimePipelinePart<T>
	{
		readonly IRuntimeContentWriter<T> _writer;
		readonly IRuntimeContent<T> _selector;

		public RuntimePipelinePart(IRuntimeContentWriter<T> writer, IRuntimeContent<T> selector)
		{
			_writer = writer;
			_selector = selector;
		}

		public bool IsSatisfiedBy(object parameter) => _writer.IsSatisfiedBy(parameter);

		public void Execute(Writing<T> parameter)
		{
			_writer.Execute(parameter);
		}

		public IMember Get() => _writer.Get();

		public IRuntimeContentWriter<T> Get(Writing<T> parameter) => _selector.Get(parameter);
	}

	sealed class RuntimeMemberedContentWriter<T> : IMemberedContentWriter<T>
	{
		readonly IParameterizedSource<Writing<T>, ImmutableArray<IContentWriter<T>>> _writers;

		public RuntimeMemberedContentWriter(IRuntimeMemberContentWriters<T> writers) => _writers = writers;

		public void Execute(Writing<T> parameter)
		{
			var members = _writers.Get(parameter);
			var length = members.Length;
			for (var i = 0; i < length; i++)
			{
				members[i].Execute(parameter);
			}
		}
	}

	interface IMemberContentWriter<T> : IContentWriter<T>, ISource<IMember> {}

	class MemberContentWriter<T> : FixedInstanceSource<IMember>, IMemberContentWriter<T>
	{
		readonly IContentWriter<T> _writer;

		public MemberContentWriter(IContentWriter<T> writer, IMember instance) : base(instance) => _writer = writer;

		public void Execute(Writing<T> parameter)
		{
			_writer.Execute(parameter);
		}
	}

	interface IMemberedContentWriter<T> : IContentWriter<T> { }

	sealed class MemberedContentWriter<T> : IMemberedContentWriter<T>
	{
		readonly ImmutableArray<IContentWriter<T>> _writers;
		readonly int _length;

		public MemberedContentWriter(IContentWriters<T> writers) : this(writers.Get().CastArray<IContentWriter<T>>()) {}

		public MemberedContentWriter(ImmutableArray<IContentWriter<T>> writers) : this(writers, writers.Length) {}

		public MemberedContentWriter(ImmutableArray<IContentWriter<T>> writers, int length)
		{
			_writers = writers;
			_length = length;
		}

		public void Execute(Writing<T> parameter)
		{
			for (var i = 0; i < _length; i++)
			{
				_writers[i].Execute(parameter);
			}
		}
	}

}