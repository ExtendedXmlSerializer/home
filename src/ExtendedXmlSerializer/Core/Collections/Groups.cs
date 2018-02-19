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
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.Core.Collections
{
	public class GroupNames : TableValueSource<string, GroupName>
	{
		public GroupNames(params GroupName[] names) : this(names.ToOrderedDictionary(x => x.Name)) {}

		public GroupNames(IDictionary<string, GroupName> store) : base(store) {}
	}

	sealed class DeclaredGroupNames<T> : InstanceMetadata<GroupElementAttribute, T, string>
	{
		public static DeclaredGroupNames<T> Default { get; } = new DeclaredGroupNames<T>();
		DeclaredGroupNames() { }
	}


	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GroupElementAttribute : Attribute, ISource<string>
	{
		readonly string _name;

		public GroupElementAttribute(string name) => _name = name;

		public string Get() => _name;
	}

	sealed class DeclaredGroupIndexes<T> : InstanceMetadataValue<InsertGroupElementAttribute, T, int>
	{
		public static DeclaredGroupIndexes<T> Default { get; } = new DeclaredGroupIndexes<T>();
		DeclaredGroupIndexes() { }
	}


	[AttributeUsage(AttributeTargets.Class)]
	public sealed class InsertGroupElementAttribute : Attribute, ISource<int>
	{
		readonly int _index;
		public InsertGroupElementAttribute() : this(0) {}

		public InsertGroupElementAttribute(int index) => _index = index;
		public int Get() => _index;
	}

	public interface IGroupName<in T> : IParameterizedSource<T, GroupName> {}

	class MetadataGroupName<T> : SpecificationSource<T, GroupName>, IGroupName<T>
	{
		public MetadataGroupName(ISpecificationSource<string, GroupName> names)
			: this(DeclaredGroupNames<T>.Default, names) {}

		public MetadataGroupName(ISpecificationSource<T, string> name, ISpecificationSource<string, GroupName> names)
			: base(name.And(names.To(name)), names.In(name)) {}
	}

	class AddCommand<T> : DelegatedCommand<T>
	{
		public AddCommand(ICollection<T> collection) : base(collection.Add) {}
	}

	class RemoveCommand<T> : ICommand<T>
	{
		readonly ICollection<T> _collection;

		public RemoveCommand(ICollection<T> collection) => _collection = collection;

		public void Execute(T parameter)
		{
			_collection.Remove(parameter);
		}
	}

	class InsertItemCommand<T> : ICommand<T>
	{
		readonly IList<T> _list;
		readonly Func<T, int> _index;

		public InsertItemCommand(IList<T> list, Func<T, int> index)
		{
			_list = list;
			_index = index;
		}

		public void Execute(T parameter)
		{
			_list.Insert(_index(parameter), parameter);
		}
	}

	sealed class AddItemCommands<T> : DelegatedSource<IList<T>, ICommand<T>>
	{
		public static AddItemCommands<T> Default { get; } = new AddItemCommands<T>();
		AddItemCommands() : base(key => new AddCommand<T>(key)) {}
	}

	sealed class ItemCommands<T> : ReferenceCache<IList<T>, ICommand<T>>
	{
		public static ItemCommands<T> Default { get; } = new ItemCommands<T>();
		ItemCommands() : base(AddItemCommands<T>.Default.Into(InsertItemCommands<T>.Default).Get) {}
	}

	sealed class InsertItemCommands<T> : IDecoration<IList<T>, ICommand<T>>
	{
		public static InsertItemCommands<T> Default { get; } = new InsertItemCommands<T>();
		InsertItemCommands() : this(DeclaredGroupIndexes<T>.Default, DeclaredGroupIndexes<T>.Default.Get) {}

		readonly ISpecification<T> _specification;
		readonly Func<T, int>      _index;

		public InsertItemCommands(ISpecification<T> specification, Func<T, int> index)
		{
			_specification = specification;
			_index         = index;
		}

		public ICommand<T> Get(Decoration<IList<T>, ICommand<T>> parameter)
			=> parameter.Result.Unless(_specification, new InsertItemCommand<T>(parameter.Parameter, _index));
	}

	public interface IGroupCollectionAware<T> : ICommand<IGroupCollection<T>> {}

	sealed class GroupingAwareCommand<T> : ICommand<IGroupCollectionAware<T>>
	{
		readonly IGroupCollection<T> _collection;

		public GroupingAwareCommand(IGroupCollection<T> collection) => _collection = collection;

		public void Execute(IGroupCollectionAware<T> parameter)
		{
			parameter.Execute(_collection);
		}
	}

	public interface IGroupNameAware : ISource<GroupName> {}

	class DefaultAddGroupElementCommand<T> : DecoratedCommand<T>
	{
		public DefaultAddGroupElementCommand(GroupName defaultName, ISpecificationSource<string, GroupName> names,
		                                     IGroupCollection<T> collection)
			: base(new AddGroupElementCommand<T>(collection, new GroupName<T>(defaultName, names))
				       .Unless(A<IGroupCollectionAware<T>>.Default, new GroupingAwareCommand<T>(collection))) {}
	}

	sealed class GroupName<T> : DecoratedSource<T, GroupName>, IGroupName<T>
	{
		public GroupName(GroupName defaultName, ISpecificationSource<string, GroupName> names)
			: base(Assume<T>.Default(defaultName).Unless(new MetadataGroupName<T>(names))) {}
	}

	sealed class AddGroupElementCommand<T> : ICommand<T>
	{
		readonly IParameterizedSource<T, ICommand<T>> _commands;

		public AddGroupElementCommand(IGroupCollection<T> collection, IParameterizedSource<T, GroupName> name)
			: this(ItemCommands<T>.Default.In(collection.In(name))) {}

		public AddGroupElementCommand(IParameterizedSource<T, ICommand<T>> commands) => _commands = commands;

		public void Execute(T parameter)
		{
			var command = _commands.Get(parameter) ?? throw new InvalidOperationException($"Could not locate a command from {parameter}.");
			command.Execute(parameter);
		}
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