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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	interface ITypedSortOrder : ITypedTable<int> {}

	public interface IGroup<T> : ICollection<T>
	{
		GroupName Name { get; }
	}

	public class Group<T> : List<T>, IGroup<T>
	{
		public Group(GroupName name) : this(name, Enumerable.Empty<T>()) {}

		public Group(GroupName name, params T[] items) : this(name, items.AsEnumerable()) {}
		public Group(GroupName name, IEnumerable<T> collection) : base(collection) => Name = name;

		public GroupName Name { get; }
	}


	public struct GroupName
	{
		public GroupName(string name) => Name = name;

		public string Name { get; }

		public bool Equals(GroupName other) => string.Equals(Name, other.Name);

		public override bool Equals(object obj) => !ReferenceEquals(null, obj) && obj is GroupName phase && Equals(phase);

		public override int GetHashCode() => Name != null ? Name.GetHashCode() : 0;
	}

	public interface IGroupContainer<T> : IEnumerable<T>, IParameterizedSource<GroupName, ICollection<T>> {}

	public interface IGroupPairing<T> : IParameterizedSource<IGroup<T>, KeyValuePair<GroupName, ICollection<T>>> {}

	sealed class GroupPairs<T> : IGroupPairing<T>
	{
		public static GroupPairs<T> Default { get; } = new GroupPairs<T>();
		GroupPairs() {}

		public KeyValuePair<GroupName, ICollection<T>> Get(IGroup<T> parameter)
			=> Pairs.Create(parameter.Name, (ICollection<T>)parameter);
	}

	public class GroupContainer<T> : DelegatedSource<GroupName, ICollection<T>>, IGroupContainer<T>
	{
		readonly IOrderedDictionary<GroupName, ICollection<T>> _store;

		public GroupContainer(IEnumerable<IGroup<T>> groups)
			: this(groups, GroupPairs<T>.Default) {}

		public GroupContainer(IEnumerable<IGroup<T>> groups, IGroupPairing<T> pairing)
			: this(new OrderedDictionary<GroupName, ICollection<T>>(groups.Select(pairing.Get))) {}

		public GroupContainer(IOrderedDictionary<GroupName, ICollection<T>> store) : base(store.GetValue) => _store = store;


		public IEnumerator<T> GetEnumerator() => _store.SelectMany(x => x.Value)
		                                               .GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	// ReSharper disable once PossibleInfiniteInheritance
	class Groups<T> : ItemsBase<IGroup<T>>
	{
		readonly ImmutableArray<GroupName> _phases;
		readonly Func<GroupName, IGroup<T>> _factory;

		public Groups(IEnumerable<GroupName> phases) : this(phases, x => new Group<T>(x)) {}

		public Groups(IEnumerable<GroupName> phases, Func<GroupName, IGroup<T>> factory)
			: this(phases.ToImmutableArray(), factory) {}

		public Groups(ImmutableArray<GroupName> phases, Func<GroupName, IGroup<T>> factory)
		{
			_phases = phases;
			_factory = factory;
		}

		public override IEnumerator<IGroup<T>> GetEnumerator() => _phases.Select(_factory)
		                                                                 .GetEnumerator();
	}
}