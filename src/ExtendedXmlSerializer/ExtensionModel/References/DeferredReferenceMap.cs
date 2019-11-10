using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System.Collections.Generic;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredReferenceMap : IReferenceMap
	{
		readonly ICollection<IDeferredCommand> _commands;
		readonly Stack<IInnerContent>          _contexts;
		readonly IReferenceMap                 _map;

		public DeferredReferenceMap(ICollection<IDeferredCommand> commands, Stack<IInnerContent> contexts,
		                            IReferenceMap map)
		{
			_commands = commands;
			_contexts = contexts;
			_map      = map;
		}

		public bool IsSatisfiedBy(ReferenceIdentity parameter) => _map.IsSatisfiedBy(parameter);

		public object Get(ReferenceIdentity parameter)
		{
			var result = _map.Get(parameter);
			if (result == null)
			{
				var source  = _map.FixedSelection(parameter);
				var current = _contexts.Peek();
				var command = current is IListInnerContent list
					              ? new DeferredCollectionAssignmentCommand(list.List, source)
					              : Member(current, source);
				_commands.Add(command);
			}

			return result;
		}

		static IDeferredCommand Member(IInnerContent current, ISource<object> source)
			=> new DeferredMemberAssignmentCommand(current.Current,
			                                       ContentsContext.Default.Get(current)
			                                                      .AsValid<IMemberAccess>(),
			                                       source);

		public void Assign(ReferenceIdentity key, object value) => _map.Assign(key, value);

		public bool Remove(ReferenceIdentity key) => _map.Remove(key);
	}
}