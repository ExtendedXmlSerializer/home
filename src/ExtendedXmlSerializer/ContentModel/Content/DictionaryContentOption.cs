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
	sealed class DictionaryContentOption : ContentOptionBase
	{
		readonly static AllSpecification<TypeInfo> Specification =
			new AllSpecification<TypeInfo>(IsActivatedTypeSpecification.Default, IsDictionaryTypeSpecification.Default);

		readonly IActivation _activation;
		readonly IMembers _members;
		readonly IDictionaryEntries _entries;
		
		public DictionaryContentOption(IActivation activation, IMembers members, IDictionaryEntries entries)
			: base(Specification)
		{
			_activation = activation;
			_members = members;
			_entries = entries;
		}

		public override ISerializer Get(TypeInfo parameter)
		{
			var members = _members.Get(parameter);
			var activator = _activation.Get(parameter);
			var entry = _entries.Get(parameter);
			var reader = new DictionaryContentsReader(activator, entry, members.ToDictionary(x => x.Adapter.Name));
			var writer = new MemberedCollectionWriter(new MemberListWriter(members), new DictionaryEntryWriter(entry));
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}