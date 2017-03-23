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
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.TypeModel;

namespace ExtendedXmlSerializer.ContentModel.Collections
{
	sealed class CollectionContentOption : CollectionContentOptionBase
	{
		readonly IMemberSerializations _members;
		readonly IEnumerators _enumerators;
		readonly IActivation _activation;

		public CollectionContentOption(IActivatingTypeSpecification specification, IActivation activation,
		                               IMemberSerializations members, IEnumerators enumerators, ISerializers serializers)
			: base(specification, serializers)
		{
			_members = members;
			_enumerators = enumerators;
			_activation = activation;
		}

		protected override ISerializer Create(ISerializer item, TypeInfo classification, TypeInfo itemType)
		{
			var members = _members.Get(classification);
			var activator = _activation.Get(classification);

			var items = new CollectionItemReader(item);
			var membered = new MemberedCollectionItemReader(items, members);

			var attributes = new MemberAttributesReader(activator, members);
			var reader = new CollectionContentsReader(attributes, membered);

			var writer = new MemberedCollectionWriter(new MemberListWriter(members), new EnumerableWriter(_enumerators, item));
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}