using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ReferenceWalker : IParameterizedSource<object, ReferenceResult>
	{
		readonly IReferencesPolicy _policy;
		readonly ProcessReference  _process;

		public ReferenceWalker(IReferencesPolicy policy, ProcessReference process)
		{
			_policy  = policy;
			_process = process;
		}

		public ReferenceResult Get(object parameter)
		{
			var result = new ReferenceSet(_policy);
			_process.Execute(new (result, parameter));
			return result;
		}
	}
}