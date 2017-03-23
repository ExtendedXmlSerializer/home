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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class DeferredReferencesExtension : ISerializerExtension
	{
		public static DeferredReferencesExtension Default { get; } = new DeferredReferencesExtension();
		DeferredReferencesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IContents, DeferredReferenceContents>()
			            .Decorate<ISerializers, DeferredReferenceSerializers>()
			            .Decorate<IReferenceEncounters, DeferredReferenceEncounters>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}

	sealed class DeferredReferenceSerializers : CacheBase<TypeInfo, ISerializer>, ISerializers
	{
		readonly ISerializers _serializers;

		public DeferredReferenceSerializers(ISerializers serializers)
		{
			_serializers = serializers;
		}

		protected override ISerializer Create(TypeInfo parameter)
			=> new DeferredReferenceSerializer(_serializers.Get(parameter));
	}

	sealed class DeferredReferenceSerializer : ISerializer
	{
		readonly IReservedItems _reserved;
		readonly ISerializer _serializer;

		public DeferredReferenceSerializer(ISerializer serializer) : this(ReservedItems.Default, serializer) {}

		public DeferredReferenceSerializer(IReservedItems reserved, ISerializer serializer)
		{
			_reserved = reserved;
			_serializer = serializer;
		}

		public object Get(IXmlReader parameter) => _serializer.Get(parameter);

		public void Write(IXmlWriter writer, object instance)
		{
			var lists = _reserved.Get(writer);
			foreach (var o in Yield(instance))
			{
				var reserved = lists.Get(o);
				if (reserved.Any())
				{
					reserved.Pop();
				}
			}

			_serializer.Write(writer, instance);
		}

		static IEnumerable<object> Yield(object instance)
		{
			if (instance is DictionaryEntry)
			{
				var entry = (DictionaryEntry) instance;
				yield return entry.Key;
				yield return entry.Value;
			}
			else
			{
				yield return instance;
			}
		}
	}

	sealed class DeferredReferenceContents : IContents
	{
		readonly static IsCollectionTypeSpecification IsCollectionTypeSpecification = IsCollectionTypeSpecification.Default;

		readonly ISpecification<TypeInfo> _specification;
		readonly IRootReferences _references;
		readonly IContents _contents;

		[UsedImplicitly]
		public DeferredReferenceContents(IRootReferences references, IContents contents)
			: this(IsCollectionTypeSpecification, references, contents) {}

		public DeferredReferenceContents(ISpecification<TypeInfo> specification, IRootReferences references,
		                                 IContents contents)
		{
			_specification = specification;
			_references = references;
			_contents = contents;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var serializer = _contents.Get(parameter);
			var result = serializer is ReferenceSerializer && _specification.IsSatisfiedBy(parameter)
				? new DeferredReferenceContent(_references, serializer)
				: serializer;
			return result;
		}
	}

	sealed class DeferredReferenceContent : ISerializer
	{
		readonly IReservedItems _reserved;
		readonly IRootReferences _references;
		readonly ISerializer _serializer;

		public DeferredReferenceContent(IRootReferences references, ISerializer serializer)
			: this(ReservedItems.Default, references, serializer) {}

		public DeferredReferenceContent(IReservedItems reserved, IRootReferences references, ISerializer serializer)
		{
			_reserved = reserved;
			_references = references;
			_serializer = serializer;
		}

		public object Get(IXmlReader parameter) => _serializer.Get(parameter);

		public void Write(IXmlWriter writer, object instance)
		{
			var list = instance as IList;
			if (list != null)
			{
				var references = _references.Get(writer);
				var hold = _reserved.Get(writer);
				foreach (var item in list)
				{
					if (references.Contains(item))
					{
						hold.Get(item).Push(list);
					}
				}
			}

			_serializer.Write(writer, instance);
		}
	}

	public interface IReservedItems : IParameterizedSource<IXmlWriter, ITrackedLists> {}

	sealed class ReservedItems : ReferenceCache<IXmlWriter, ITrackedLists>, IReservedItems
	{
		public static ReservedItems Default { get; } = new ReservedItems();
		ReservedItems() : base(_ => new TrackedLists()) {}
	}

	public interface ITrackedLists : IParameterizedSource<object, Stack<IList>> {}

	sealed class TrackedLists : Cache<object, Stack<IList>>, ITrackedLists
	{
		public TrackedLists() : base(_ => new Stack<IList>()) {}
	}

	sealed class DeferredReferenceEncounters : IReferenceEncounters
	{
		readonly IReferenceEncounters _encounters;
		readonly IReservedItems _reserved;

		public DeferredReferenceEncounters(IReferenceEncounters encounters) : this(encounters, ReservedItems.Default) {}

		public DeferredReferenceEncounters(IReferenceEncounters encounters, IReservedItems reserved)
		{
			_encounters = encounters;
			_reserved = reserved;
		}

		public IEncounters Get(IXmlWriter parameter)
			=> new DeferredEncounters(_encounters.Get(parameter), _reserved.Get(parameter));
	}

	/*sealed class TypedEnumerators : IEnumeratorStore
	{
		readonly IEnumeratorStore _store;

		public TypedEnumerators(IEnumeratorStore store)
		{
			_store = store;
		}

		public IEnumerators Get(TypeInfo parameter) => new Enumerators(_store.Get(parameter));
	}

	class Enumerators : IEnumerators
	{
		readonly IEnumerators _enumerators;

		public Enumerators(IEnumerators enumerators)
		{
			_enumerators = enumerators;
		}

		public IEnumerator Get(IEnumerable parameter) => _enumerators.Get(parameter);
	}*/

	sealed class DeferredEncounters : IEncounters
	{
		readonly IEncounters _encounters;
		readonly ITrackedLists _tracked;

		public DeferredEncounters(IEncounters encounters, ITrackedLists tracked)
		{
			_encounters = encounters;
			_tracked = tracked;
		}

		public bool IsSatisfiedBy(object parameter) => !_tracked.Get(parameter).Any() && _encounters.IsSatisfiedBy(parameter);

		public Identifier? Get(object parameter)
		{
			var result = _encounters.Get(parameter);
			return result;
		}
	}
}