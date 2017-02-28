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
using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ExtensionModel
{
	public interface ISerializerExtension : IAlteration<IServices> {}


	class ClassicExtension : ISerializerExtension
	{
		public static ClassicExtension Default { get; } = new ClassicExtension();
		ClassicExtension() {}

		public IServices Get(IServices parameter)
			=>
				parameter.Register<IContentOptions, ClassicContentOptions>()
				         .RegisterInstance(ClassicEmitMemberSpecifications.Default);
	}

	class ClassicEmitMemberSpecifications : IMemberEmitSpecifications
	{
		readonly static AlwaysEmitMemberSpecification Always = AlwaysEmitMemberSpecification.Default;

		public static ClassicEmitMemberSpecifications Default { get; } = new ClassicEmitMemberSpecifications();
		ClassicEmitMemberSpecifications() : this(IsAssignableSpecification<Enum>.Default) {}

		readonly ISpecification<TypeInfo> _valueType;

		public ClassicEmitMemberSpecifications(ISpecification<TypeInfo> valueType)
		{
			_valueType = valueType;
		}

		public IMemberEmitSpecification Get(MemberDescriptor parameter)
			=> _valueType.IsSatisfiedBy(parameter.MemberType) ? Always : null;
	}


	class ClassicCollectionContentOption : CollectionContentOptionBase
	{
		readonly IActivation _activation;

		public ClassicCollectionContentOption(IActivation activation, ISerialization serialization)
			: base(serialization)
		{
			_activation = activation;
		}

		protected sealed override ISerializer Create(ISerializer item, TypeInfo classification, TypeInfo itemType)
			=>
				new Serializer(new CollectionContentsReader(_activation.Get(classification), item, itemType),
				               new EnumerableWriter(item));
	}


	class ClassicContentOptions : IContentOptions
	{
		readonly ISerialization _owner;
		readonly IMembers _members;
		readonly IMemberOption _variable;
		readonly ISerializer _runtime;
		readonly IActivation _activation;
		readonly IMemberSerialization _memberSerialization;

		public ClassicContentOptions(
			IActivation activation,
			ISerialization owner,
			IMemberSerialization memberSerialization,
			IMembers members,
			IMemberOption variable, ISerializer runtime)
		{
			_owner = owner;
			_memberSerialization = memberSerialization;
			_members = members;
			_variable = variable;
			_runtime = runtime;
			_activation = activation;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public IEnumerator<IContentOption> GetEnumerator()
		{
			yield return new ArrayContentOption(_activation, _owner);
			var entries = new DictionaryEntries(_activation, _memberSerialization, _variable);
			yield return new ClassicDictionaryContentOption(_activation, _members, entries);
			yield return new ClassicCollectionContentOption(_activation, _owner);
			yield return new MemberedContentOption(_activation, _members);
			yield return new RuntimeContentOption(_runtime);
		}
	}

	class ClassicDictionaryContentOption : ContentOptionBase
	{
		readonly static AllSpecification<TypeInfo> Specification =
			new AllSpecification<TypeInfo>(IsActivatedTypeSpecification.Default, IsDictionaryTypeSpecification.Default);

		readonly IActivation _activation;
		readonly IMembers _members;
		readonly IDictionaryEntries _entries;

		public ClassicDictionaryContentOption(IActivation activation, IMembers members, IDictionaryEntries entries)
			: base(Specification)
		{
			_activation = activation;
			_members = members;
			_entries = entries;
		}

		public sealed override ISerializer Get(TypeInfo parameter)
		{
			var activator = _activation.Get(parameter);
			var entry = _entries.Get(parameter);
			var reader = new DictionaryContentsReader(activator, entry, _members.Get(parameter).ToDictionary(x => x.Adapter.Name));
			var result = new Serializer(reader, new DictionaryEntryWriter(entry));
			return result;
		}

		class DictionaryContentsReader : CollectionContentsReader
		{
			readonly static ILists Lists = new Lists(DictionaryAddDelegates.Default);
			readonly static TypeInfo ItemType = DictionaryEntries.Type;

			public DictionaryContentsReader(IReader reader, IReader entry, IDictionary<string, IMember> members)
				: base(new MemberAttributesReader(reader, members),
				       new CollectionItemReader(entry), Lists, ItemType) {}
		}
	}
}