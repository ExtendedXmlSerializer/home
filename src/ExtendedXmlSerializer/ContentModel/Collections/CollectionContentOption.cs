// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
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
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ContentModel.Collections
{
	sealed class CollectionContentOption : CollectionContentOptionBase
	{
		readonly IMemberSerializations _members;
		readonly IActivation _activation;

		public CollectionContentOption(IActivatingTypeSpecification specification, IActivation activation,
		                               IMemberSerializations members, ISerializers serializers)
			: base(specification, serializers)
		{
			_members = members;
			_activation = activation;
		}

		protected override ISerializer Create(ISerializer item, TypeInfo classification)
		{
			var members = _members.Get(classification);
			var activator = _activation.Get(classification);

			var items = new CollectionItemReader(item);
			var membered = new MemberedCollectionItemReader(items, members);

			var attributes = new MemberAttributesReader(activator, members);
			var reader = new CollectionContentsReader(attributes, membered);

			var writer = new MemberedCollectionWriter(new MemberListWriter(members), new EnumerableWriter(item));
			var result = new Serializer(reader, writer);
			return result;
		}
	}
}