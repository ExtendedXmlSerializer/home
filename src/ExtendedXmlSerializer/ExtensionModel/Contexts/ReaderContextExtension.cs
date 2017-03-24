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
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Xml;
using ExtendedXmlSerializer.Core;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ExtensionModel.Contexts
{
	public sealed class ReaderContextExtension : ISerializerExtension
	{
		public static ReaderContextExtension Default { get; } = new ReaderContextExtension();
		ReaderContextExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IMemberAssignment, MemberAssignment>()
			            .Decorate<ICollectionAssignment, CollectionAssignment>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class MemberAssignment : IMemberAssignment
		{
			readonly IReaderContexts _contexts;
			readonly IMemberAssignment _assignment;

			[UsedImplicitly]
			public MemberAssignment(IMemberAssignment assignment) : this(ReaderContexts.Default, assignment) {}

			public MemberAssignment(IReaderContexts contexts, IMemberAssignment assignment)
			{
				_contexts = contexts;
				_assignment = assignment;
			}

			public void Assign(IXmlReader context, IReader reader, object instance, IMemberAccess access)
			{
				var contexts = _contexts.Get(context);
				contexts.Push(new MemberReadContext(instance, access));
				_assignment.Assign(context, reader, instance, access);
				contexts.Pop();
			}

			public object Complete(IXmlReader context, object instance) => _assignment.Complete(context, instance);
		}

		sealed class CollectionAssignment : ICollectionAssignment
		{
			readonly IReaderContexts _contexts;
			readonly ICollectionAssignment _assignment;

			[UsedImplicitly]
			public CollectionAssignment(ICollectionAssignment assignment) : this(ReaderContexts.Default, assignment) {}

			public CollectionAssignment(IReaderContexts contexts, ICollectionAssignment assignment)
			{
				_contexts = contexts;
				_assignment = assignment;
			}

			public void Assign(IXmlReader reader, object instance, IList list, object item)
			{
				var contexts = _contexts.Get(reader);
				var context = new CollectionReadContext(instance, list, item);
				contexts.Push(context);
				_assignment.Assign(reader, instance, list, item);
				contexts.Pop();
			}

			public object Complete(IXmlReader reader, object instance, IList list)
			{
				return _assignment.Complete(reader, instance, list);
			}
		}
	}
}