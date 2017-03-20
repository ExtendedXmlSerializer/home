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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class DictionaryEntries : IDictionaryEntries
	{
		readonly static TypeInfo Type = typeof(DictionaryEntry).GetTypeInfo();

		readonly static PropertyInfo
			Key = Type.GetProperty(nameof(DictionaryEntry.Key)),
			Value = Type.GetProperty(nameof(DictionaryEntry.Value));

		readonly static DictionaryPairTypesLocator Pairs = DictionaryPairTypesLocator.Default;

		readonly IReader _activator;
		readonly IMembers _profiles;
		readonly IMemberSerializers _serializers;
		readonly IWriter _element;
		readonly IDictionaryPairTypesLocator _locator;

		[UsedImplicitly]
		public DictionaryEntries(Xml.IIdentities identities, IActivation activation, IMembers profiles,
		                         IMemberSerializers serializers)
			: this(activation.Get<DictionaryEntry>(), profiles, serializers, new ElementOption(identities).Get(Type), Pairs) {}

		public DictionaryEntries(IReader activator, IMembers profiles, IMemberSerializers serializers, IWriter element,
		                         IDictionaryPairTypesLocator locator)
		{
			_activator = activator;
			_profiles = profiles;
			_serializers = serializers;
			_element = element;
			_locator = locator;
		}

		IMemberSerializer Create(PropertyInfo metadata, TypeInfo classification)
			=> _serializers.Get(_profiles.Get(new MemberDescriptor(metadata, classification)));

		public ISerializer Get(TypeInfo parameter)
		{
			var pair = _locator.Get(parameter);
			var members = new[] {Create(Key, pair.KeyType), Create(Value, pair.ValueType)}.ToImmutableArray();
			var serialization = new MemberSerialization(new FixedRuntimeMemberList(members),
			                                            members.ToDictionary(x => x.Profile.Name, x => x), members);
			var reader = new MemberContentsReader(_activator, serialization);

			var converter = new Serializer(reader, new MemberListWriter(serialization));
			var result = new Container(_element, converter);
			return result;
		}
	}
}