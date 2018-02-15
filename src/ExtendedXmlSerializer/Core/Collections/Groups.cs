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
using System.Reflection;

namespace ExtendedXmlSerializer.Core.Collections
{
	public class GroupNames : TableValueSource<string, GroupName>
	{
		public GroupNames(params GroupName[] names) : this(names.ToOrderedDictionary(x => x.Name)) {}

		public GroupNames(IDictionary<string, GroupName> store) : base(store) {}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GroupElementAttribute : Attribute, ISource<string>
	{
		public static IParameterizedSource<TypeInfo, string> Names { get; } =
			new TypeMetadataValue<GroupElementAttribute, string>().ReferenceCache();

		readonly string _name;

		public GroupElementAttribute(string name) => _name = name;

		public string Get() => _name;
	}

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class InsertGroupElementAttribute : Attribute, ISource<int?>
	{
		public static IParameterizedSource<TypeInfo, int?> Indexes { get; } =
			new StructureCache<TypeInfo, int?>(new TypeMetadataValue<InsertGroupElementAttribute, int?>().Get);


		readonly int _index;
		public InsertGroupElementAttribute() : this(0) {}

		public InsertGroupElementAttribute(int index) => _index = index;
		public int? Get() => _index;
	}

	public interface IGroupNameAware : ISource<GroupName> { }

	public interface IGroupName<in T> : IParameterizedSource<T, GroupName?> {}

	class GroupNameAware<T> : IGroupName<T>
	{
		public static GroupNameAware<T> Default { get; } = new GroupNameAware<T>();
		GroupNameAware() {}

		public GroupName? Get(T parameter) => parameter.To<IGroupNameAware>()
		                                               .Get();
	}

	class MetadataGroupName<T> : IGroupName<T>
	{
		readonly IParameterizedSource<TypeInfo, string> _name;
		readonly ISpecificationSource<string, GroupName> _names;

		public MetadataGroupName(ISpecificationSource<string, GroupName> names) : this(GroupElementAttribute.Names, names) {}

		public MetadataGroupName(IParameterizedSource<TypeInfo, string> name, ISpecificationSource<string, GroupName> names)
		{
			_name = name;
			_names = names;
		}

		public GroupName? Get(T parameter)
		{
			var name   = _name.Get(parameter.GetType());
			var result = name != null && _names.IsSatisfiedBy(name) ? (GroupName?)_names.Get(name) : null;
			return result;
		}
	}

	class AddItemCommand<T> : DelegatedCommand<T>
	{
		public AddItemCommand(ICollection<T> collection) : base(collection.Add) {}
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
		AddItemCommands() : base(key => new AddItemCommand<T>(key)) {}
	}

	sealed class ItemCommands<T> : ReferenceCache<IList<T>, ICommand<T>>
	{
		public static ItemCommands<T> Default { get; } = new ItemCommands<T>();
		ItemCommands() : base(AddItemCommands<T>.Default.Unless(InsertItemCommands<T>.Default).Get) {}
	}

	sealed class InsertItemCommands<T> : IDecoration<IList<T>, ICommand<T>>
	{
		public static InsertItemCommands<T> Default { get; } = new InsertItemCommands<T>();
		InsertItemCommands() : this(InsertGroupElementAttribute.Indexes.In(InstanceMetadataCoercer<T>.Default)) {}

		readonly ISpecification<T> _specification;
		readonly Func<T, int>      _index;

		public InsertItemCommands(IParameterizedSource<T, int?> source) : this(source.IfAssigned(), source.To(NullableValueCoercer<int>.Default).ToDelegate()) {}

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

	class AddGroupElementCommand<T> : DecoratedCommand<T>
	{
		public AddGroupElementCommand(GroupName defaultName, ISpecificationSource<string, GroupName> names, IGroupCollection<T> collection)
			: base(new DefaultAddGroupElementCommand<T>(defaultName, collection, new GroupName<T>(names))
				       .Unless(A<IGroupCollectionAware<T>>.Default, new GroupingAwareCommand<T>(collection))) {}
	}

	sealed class GroupName<T> : DecoratedSource<T, GroupName?>, IGroupName<T>
	{
		public GroupName(ISpecificationSource<string, GroupName> names)
			: base(new MetadataGroupName<T>(names).If(A<GroupElementAttribute>.Default)
			                                      .Unless(A<IGroupNameAware>.Default, GroupNameAware<T>.Default)) {}
	}

	sealed class DefaultAddGroupElementCommand<T> : ICommand<T>
	{
		readonly GroupName _defaultName;
		readonly IGroupCollection<T> _collection;
		readonly IGroupName<T> _name;
		readonly IParameterizedSource<IList<T>, ICommand<T>> _commands;

		public DefaultAddGroupElementCommand(GroupName defaultName, IGroupCollection<T> collection, IGroupName<T> name)
			: this(defaultName, collection, name, ItemCommands<T>.Default) {}

		public DefaultAddGroupElementCommand(GroupName defaultName, IGroupCollection<T> collection, IGroupName<T> name, IParameterizedSource<IList<T>, ICommand<T>> commands)
		{
			_defaultName = defaultName;
			_collection = collection;
			_name = name;
			_commands = commands;
		}

		public void Execute(T parameter)
		{
			var name = _name.Get(parameter) ?? _defaultName;
			var group = _collection.Get(name) ?? throw new InvalidOperationException($"Group {name.Name} was found, but an associated collection for this name could not be located.");
			_commands.Get(group)?.Execute(parameter);
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