using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class DeferredMemberAssignmentCommand : IDeferredCommand
	{
		readonly object          _instance;
		readonly IMemberAccess   _access;
		readonly ISource<object> _source;

		public DeferredMemberAssignmentCommand(object instance, IMemberAccess access, ISource<object> source)
		{
			_instance = instance;
			_access   = access;
			_source   = source;
		}

		public void Execute(object parameter) => _access.Assign(_instance, parameter);

		public object Get() => _source.Get();
	}
}