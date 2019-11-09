using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ContentModel.Content
{
	sealed class DictionaryEntries : IDictionaryEntries
	{
		readonly static TypeInfo Type = Support<DictionaryEntry>.Key;

		readonly static PropertyInfo
			Key   = Type.GetProperty(nameof(DictionaryEntry.Key)),
			Value = Type.GetProperty(nameof(DictionaryEntry.Value));

		readonly static DictionaryPairTypesLocator Pairs = DictionaryPairTypesLocator.Default;

		readonly Func<IEnumerable<IMemberSerializer>, IMemberSerialization> _builder;
		readonly IInnerContentServices                                      _contents;
		readonly IMembers                                                   _members;
		readonly IMemberSerializers                                         _serializers;
		readonly IWriter                                                    _element;
		readonly IDictionaryPairTypesLocator                                _locator;

		[UsedImplicitly]
		public DictionaryEntries(IInnerContentServices contents, Element element, IMembers members,
		                         IMemberSerializers serializers)
			: this(MemberSerializationBuilder.Default.Get, contents, serializers, members, element.Get(Type), Pairs) {}

		public DictionaryEntries(Func<IEnumerable<IMemberSerializer>, IMemberSerialization> builder,
		                         IInnerContentServices contents, IMemberSerializers serializers, IMembers members,
		                         IWriter element, IDictionaryPairTypesLocator locator)
		{
			_builder     = builder;
			_contents    = contents;
			_members     = members;
			_serializers = serializers;
			_element     = element;
			_locator     = locator;
		}

		IMemberSerializer Create(PropertyInfo metadata, TypeInfo classification)
			=> _serializers.Get(_members.Get(new MemberDescriptor(metadata, classification)));

		public ISerializer Get(TypeInfo parameter)
		{
			var pair          = _locator.Get(parameter);
			var serializers   = new[] {Create(Key, pair.KeyType), Create(Value, pair.ValueType)};
			var serialization = new FixedInstanceMemberSerialization(_builder(serializers));

			var reader = _contents.Create(Type, new MemberInnerContentHandler(serialization, _contents, _contents));

			var converter = new Serializer(reader, new MemberListWriter(serialization));
			var result    = new Container<object>(_element, converter);
			return result;
		}
	}
}