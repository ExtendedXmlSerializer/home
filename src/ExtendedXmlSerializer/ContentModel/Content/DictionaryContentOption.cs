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

using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Collections;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Content
{
	class DictionaryContentOption : ContentOptionBase
	{
		readonly static Activators Activators = Activators.Default;

		readonly static AllSpecification<TypeInfo> Specification =
			new AllSpecification<TypeInfo>(IsActivatedTypeSpecification.Default, IsDictionaryTypeSpecification.Default);

		readonly IMembers _members;
		readonly IDictionaryEntries _entries;
		readonly IActivators _activators;

		public DictionaryContentOption(IMembers members, IDictionaryEntries entries) : this(members, entries, Activators) {}

		public DictionaryContentOption(IMembers members, IDictionaryEntries entries, IActivators activators)
			: base(Specification)
		{
			_members = members;
			_entries = entries;
			_activators = activators;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var activator = new DelegatedFixedActivator(_activators.Get(parameter.AsType()));
			var entry = _entries.Get(parameter);
			var reader = new DictionaryContentsReader(activator, entry, members.ToDictionary(x => x.DisplayName));
			var writer = new MemberedCollectionWriter(new MemberListWriter(members), new DictionaryEntryWriter(entry));
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}