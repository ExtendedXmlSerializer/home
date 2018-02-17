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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ExtensionGroupCollection : IList<ISerializerExtension>
	{
		readonly ICollection<ISerializerExtension> _container;
		readonly IList<ISerializerExtension> _collection;

		[UsedImplicitly]
		public ExtensionGroupCollection(ICollection<ISerializerExtension> container, params ISerializerExtension[] items)
			: this(container.AddingAll(items), new List<ISerializerExtension>(items)) {}

		public ExtensionGroupCollection(ICollection<ISerializerExtension> container, IList<ISerializerExtension> collection)
		{
			_container = container;
			_collection = collection;
		}

		public IEnumerator<ISerializerExtension> GetEnumerator() => _collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Add(ISerializerExtension item)
		{
			_container.Add(item);
			_collection.Add(item);
		}

		public void Clear()
		{
			foreach (var extension in _collection)
			{
				_container.Remove(extension);
			}
			_collection.Clear();
		}

		public bool Contains(ISerializerExtension item) => _collection.Contains(item);

		public void CopyTo(ISerializerExtension[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		public bool Remove(ISerializerExtension item) => _collection.Remove(item) && _container.Remove(item);

		public int Count => _collection.Count;

		public bool IsReadOnly => _collection.IsReadOnly;
		public int IndexOf(ISerializerExtension item)
		{
			_container.Add(item);
			return _collection.IndexOf(item);
		}

		public void Insert(int index, ISerializerExtension item)
		{
			_container.Add(item);
			_collection.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_container.Remove(_collection[index]);
			_collection.RemoveAt(index);
		}

		public ISerializerExtension this[int index]
		{
			get => _collection[index];
			set => _collection[index] = value;
		}
	}

	sealed class Group : Group<ISerializerExtension>
	{
		public Group(GroupName name, ICollection<ISerializerExtension> container) : this(name, container, Support<ISerializerExtension>.Empty) {}
		public Group(GroupName name, ICollection<ISerializerExtension> container, params ISerializerExtension[] items)
			: base(name, new ExtensionGroupCollection(container, items)) {}

	}

	sealed class Categories : GroupNames
	{
		public static GroupName Start = new GroupName("Start"),
		                        TypeSystem = new GroupName("Type System"),
		                        ObjectModel = new GroupName("Object Model"),
								Framework = new GroupName("Framework"),
		                        Elements = new GroupName("Elements"),
		                        Content = new GroupName("Content"),
		                        Format = new GroupName("Format"),
		                        Caching = new GroupName("Caching"),
		                        Finish = new GroupName("Finish");

		public static Categories Default { get; } = new Categories();
		Categories() : this(Start, TypeSystem, Framework, Elements, Content, Format, Caching, Finish) {}

		public Categories(params GroupName[] names) : base(names) {}
	}

	sealed class DefaultGroups : ItemsBase<IGroup<ISerializerExtension>>
	{
		public static DefaultGroups Default { get; } = new DefaultGroups();
		DefaultGroups() : this(DefaultMetadataSpecification.Default, DefaultMemberOrder.Default) {}

		readonly IMetadataSpecification _metadata;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		public DefaultGroups(IMetadataSpecification metadata,
		                     IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_metadata = metadata;
			_defaultMemberOrder = defaultMemberOrder;
		}

		public override IEnumerator<IGroup<ISerializerExtension>> GetEnumerator()
		{
			var all = new KeyedByTypeCollection<ISerializerExtension>();
			yield return new Group(Categories.Start, all,
								   new ConfigurationServicesExtension()
			                      );

			yield return new Group(Categories.TypeSystem, all,
								   TypeModelExtension.Default,
			                       SingletonActivationExtension.Default,
								   TypeResolutionExtension.Default,
			                       ImmutableArrayExtension.Default,
			                       MemberModelExtension.Default,
			                       new MemberNamesExtension(),
			                       new MemberOrderingExtension(_defaultMemberOrder)
								  );
			yield return new Group(Categories.ObjectModel, all,
			                       new DefaultReferencesExtension());
			yield return new Group(Categories.Framework, all,
			                       SerializationExtension.Default);
			yield return new Group(Categories.Elements, all,
			                       ElementsExtension.Default);
			yield return new Group(Categories.Content, all,
								   ContentModelExtension.Default,
								   CommonContentExtension.Default,
			                       new AllowedMembersExtension(_metadata),
			                       new AllowedMemberValuesExtension(),
			                       new ConvertersExtension(),
			                       new RegisteredSerializersExtension()
			                      );
			yield return new Group(Categories.Format, all,
								   new XmlSerializationExtension(),
			                       new MemberFormatExtension()
			                      );
			yield return new Group(Categories.Caching, all, CachingExtension.Default);
			yield return new Group(Categories.Finish, all);
		}
	}
}