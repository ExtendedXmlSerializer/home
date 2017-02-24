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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Collections
{
	class CollectionContentOption : CollectionContentOptionBase
	{
		readonly static Activators Activators = Activators.Default;

		readonly IMembers _members;
		readonly IActivators _activators;

		public CollectionContentOption(IMembers members, ISerialization serialization)
			: this(members, serialization, Activators) {}

		public CollectionContentOption(IMembers members, ISerialization serialization, IActivators activators)
			: base(serialization)
		{
			_members = members;
			_activators = activators;
		}

		protected sealed override ISerializer Create(ISerializer item, TypeInfo classification)
		{
			var members = _members.Get(classification);
			var activator = new DelegatedFixedActivator(_activators.Get(classification.AsType()));

			var items = new CollectionItemReader(item);
			var dictionary = members.ToDictionary(x => x.Adapter.Name);
			var membered = new MemberedCollectionItemReader(items, dictionary);

			var attributes = new MemberAttributesReader(activator, dictionary);
			var reader = new CollectionContentsReader(attributes, membered);
			var writer = new MemberedCollectionWriter(new MemberListWriter(members), new EnumerableWriter(item));
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}