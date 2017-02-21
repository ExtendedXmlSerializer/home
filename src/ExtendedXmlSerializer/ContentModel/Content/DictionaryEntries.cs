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
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class DictionaryEntries : IDictionaryEntries
	{
		readonly static TypeInfo Type = typeof(DictionaryEntry).GetTypeInfo();
		readonly static PropertyInfo Key = Type.GetProperty(nameof(DictionaryEntry.Key));
		readonly static PropertyInfo Value = Type.GetProperty(nameof(DictionaryEntry.Value));
		readonly static IWriter Element = ElementOption.Default.Get(Type);
		readonly static DictionaryPairTypesLocator Pairs = DictionaryPairTypesLocator.Default;

		readonly ISerialization _serialization;
		readonly IMemberOption _variable;
		readonly IWriter _element;
		readonly IDictionaryPairTypesLocator _locator;

		public DictionaryEntries(ISerialization serialization, IMemberOption variable)
			: this(serialization, variable, Element, Pairs) {}

		public DictionaryEntries(ISerialization serialization, IMemberOption variable, IWriter element,
		                         IDictionaryPairTypesLocator locator)
		{
			_serialization = serialization;
			_variable = variable;
			_element = element;
			_locator = locator;
		}

		IMember Create(MemberInfo metadata, TypeInfo classification)
		{
			var profile = _serialization.Get(new MemberDescriptor(Type, metadata, classification, true));
			var decorated = new MemberProfile(profile.Specification, profile.Identity, profile.AllowWrite,
			                                  profile.Order, profile.Metadata, classification, profile.Reader,
			                                  profile.Writer);
			var result = _variable.Get(decorated);
			return result;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var pair = _locator.Get(parameter);
			var key = Create(Key, pair.KeyType);
			var value = Create(Value, pair.ValueType);
			var members = ImmutableArray.Create(key, value);
			var reader = new MemberContentsReader(Activator<DictionaryEntry>.Default, members.ToDictionary(x => x.DisplayName));
			var converter = new Serializer(reader, new MemberListWriter(members));
			var result = new Container(_element, converter);
			return result;
		}

/*
		class MemberProfile : IMemberProfile
		{
			readonly IMemberProfile _profile;

			public MemberProfile(IMemberProfile profile, TypeInfo memberType)
			{
				_profile = profile;
				MemberType = memberType;
			}

			public TypeInfo MemberType { get; }
			public IWriter Writer => _profile.Writer;
			public IReader Reader => _profile.Reader;

			public ISpecification<object> Specification => _profile.Specification;

			public bool AllowWrite => _profile.AllowWrite;

			public int Order => _profile.Order;

			public MemberInfo Metadata => _profile.Metadata;

			public string Name => _profile.Name;
			public string Identifier => _profile.Identifier;
			
		}
*/
	}
}