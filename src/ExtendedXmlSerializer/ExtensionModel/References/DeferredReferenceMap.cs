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

using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceMap : IReferenceMap
	{
		readonly ICollection<IDeferredCommand> _commands;
		readonly Stack<IInnerContent> _contexts;
		readonly IReferenceMap _map;

		public DeferredReferenceMap(ICollection<IDeferredCommand> commands, Stack<IInnerContent> contexts,
		                            IReferenceMap map)
		{
			_commands = commands;
			_contexts = contexts;
			_map = map;
		}

		public bool IsSatisfiedBy(ReferenceIdentity parameter) => _map.IsSatisfiedBy(parameter);

		public object Get(ReferenceIdentity parameter)
		{
			var result = _map.Get(parameter);
			if (result == null)
			{
				var source = _map.Fix(parameter);
				var current = _contexts.Peek();
				var command = Command(current, source);
				_commands.Add(command);
			}
			return result;
		}

		static IDeferredCommand Command(IInnerContent current, ISource<object> source)
		{
			var list = current as IListInnerContent;
			var result = list != null
				? new DeferredCollectionAssignmentCommand(list.List, source)
				: Member(current, source);
			return result;
		}

		static IDeferredCommand Member(IInnerContent current, ISource<object> source)
			=>
				new DeferredMemberAssignmentCommand(current.Current, ContentsContext.Default.Get(current).AsValid<IMemberAccess>(),
				                                    source);

		public void Assign(ReferenceIdentity key, object value) => _map.Assign(key, value);
		public bool Remove(ReferenceIdentity key) => _map.Remove(key);
	}
}