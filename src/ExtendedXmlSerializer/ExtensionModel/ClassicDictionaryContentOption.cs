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

using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ClassicDictionaryContentOption : ContentOptionBase
	{
		readonly IActivation _activation;
		readonly IMemberSerializations _serializations;
		readonly IDictionaryEnumerators _enumerators;
		readonly IDictionaryEntries _entries;
		readonly ICollectionAssignment _collection;
		readonly IMemberAssignment _member;

		public ClassicDictionaryContentOption(IActivatingTypeSpecification specification, IActivation activation,
		                                      IMemberSerializations serializations, IMemberAssignment member,
		                                      IDictionaryEnumerators enumerators, IDictionaryEntries entries,
		                                      ICollectionAssignment collection)
			: base(specification.And(IsDictionaryTypeSpecification.Default))
		{
			_activation = activation;
			_serializations = serializations;
			_enumerators = enumerators;
			_entries = entries;
			_collection = collection;
			_member = member;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var activator = _activation.Get(parameter);
			var entry = _entries.Get(parameter);
			var reader = new DictionaryContentsReader(activator, entry, _serializations.Get(parameter), _member, _collection);
			var result = new Serializer(reader, new EnumerableWriter(_enumerators, entry));
			return result;
		}

		sealed class DictionaryContentsReader : DecoratedReader
		{
			readonly static ILists Lists = new Lists(DictionaryAddDelegates.Default);

			public DictionaryContentsReader(IReader reader, IReader entry, IMemberSerialization members,
			                                IMemberAssignment member, ICollectionAssignment collection)
				: base(new CollectionContentsReader(new MemberAttributesReader(reader, members, member),
				                                    new CollectionReadAssignment(entry, collection), Lists)) {}
		}
	}
}