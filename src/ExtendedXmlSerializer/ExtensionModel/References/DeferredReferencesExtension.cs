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
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	public sealed class DeferredReferencesExtension : ISerializerExtension
	{
		public static DeferredReferencesExtension Default { get; } = new DeferredReferencesExtension();
		DeferredReferencesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberAssignment, MemberAssignment>()
			            .Decorate<ICollectionAssignment, CollectionAssignment>()
			            .Decorate<IReferenceMaps, DeferredReferenceMaps>()
			            .Decorate<IContents, DeferredReferenceContents>()
			            .Decorate<ISerializers, DeferredReferenceSerializers>()
			            .Decorate<IReferenceEncounters, DeferredReferenceEncounters>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class MemberAssignment : IMemberAssignment
		{
			readonly IMemberAssignment _assignment;

			[UsedImplicitly]
			public MemberAssignment(IMemberAssignment assignment)
			{
				_assignment = assignment;
			}

			public void Assign(IXmlReader context, IReader reader, object instance, IMemberAccess access)
				=> _assignment.Assign(context, reader, instance, access);

			public object Complete(IXmlReader context, object instance) => _assignment.Complete(context, instance);
		}

		sealed class CollectionAssignment : ICollectionAssignment
		{
			readonly ICommand<IXmlReader> _member;
			readonly ICommand<IXmlReader> _collection;
			readonly ICollectionAssignment _assignment;

			[UsedImplicitly]
			public CollectionAssignment(ICollectionAssignment assignment)
				: this(
					  ExecuteDeferredCommandsCommand<DeferredMemberAssignmentCommand>.Default,
					  ExecuteDeferredCommandsCommand<DeferredCollectionAssignmentCommand>.Default, assignment) {}

			public CollectionAssignment(ICommand<IXmlReader> member, ICommand<IXmlReader> collection, ICollectionAssignment assignment)
			{
				_member = member;
				_collection = collection;
				_assignment = assignment;
			}

			public void Assign(IXmlReader reader, object instance, IList list, object item)
				=> _assignment.Assign(reader, instance, list, item);

			public object Complete(IXmlReader reader, object instance, IList list)
			{
				_member.Execute(reader);
				_collection.Execute(reader);
				var result = _assignment.Complete(reader, instance, list);
				return result;
			}
		}
	}
}