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

using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class DictionaryEntries : IDictionaryEntries
	{
		readonly static TypeInfo Type = Support<DictionaryEntry>.Key;

		readonly static PropertyInfo
			Key = Type.GetProperty(nameof(DictionaryEntry.Key)),
			Value = Type.GetProperty(nameof(DictionaryEntry.Value));

		readonly static DictionaryPairTypesLocator Pairs = DictionaryPairTypesLocator.Default;

		readonly Func<IEnumerable<IMemberSerializer>, IMemberSerialization> _builder;
		readonly IInnerContentServices _contents;
		readonly IMembers _members;
		readonly IMemberSerializers _serializers;
		readonly IWriter _element;
		readonly IDictionaryPairTypesLocator _locator;

		[UsedImplicitly]
		public DictionaryEntries(IInnerContentServices contents, IIdentities identities, IMembers members,
		                         IMemberSerializers serializers)
			: this(MemberSerializationBuilder.Default.Get, contents, serializers, members, new ElementOption(identities).Get(Type), Pairs) {}

		public DictionaryEntries(Func<IEnumerable<IMemberSerializer>, IMemberSerialization> builder,
		                         IInnerContentServices contents, IMemberSerializers serializers, IMembers members,
		                         IWriter element, IDictionaryPairTypesLocator locator)
		{
			_builder = builder;
			_contents = contents;
			_members = members;
			_serializers = serializers;
			_element = element;
			_locator = locator;
		}

		IMemberSerializer Create(PropertyInfo metadata, TypeInfo classification)
			=> _serializers.Get(_members.Get(new MemberDescriptor(metadata, classification)));

		public ISerializer Get(TypeInfo parameter)
		{
			var pair = _locator.Get(parameter);
			var serializers = new[] { Create(Key, pair.KeyType), Create(Value, pair.ValueType) };
			var serialization = _builder(serializers);

			var reader = _contents.Create(Type, new MemberInnerContentHandler(serialization, _contents, _contents));

			var converter = new Serializer(reader, new MemberListWriter(serialization));
			var result = new Container(_element, converter);
			return result;
		}
	}
}