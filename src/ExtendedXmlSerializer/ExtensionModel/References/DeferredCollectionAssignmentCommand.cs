using System.Collections;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredCollectionAssignmentCommand : IDeferredCommand
	{
		readonly IList           _list;
		readonly int             _index;
		readonly ISource<object> _source;

		public DeferredCollectionAssignmentCommand(IList list, ISource<object> source) :
			this(list, list.Count, source) {}

		public DeferredCollectionAssignmentCommand(IList list, int index, ISource<object> source)
		{
			_list   = list;
			_index  = index;
			_source = source;
		}

		public void Execute(object parameter) => _list[_index] = parameter;

		public object Get() => _source.Get();
	}
}