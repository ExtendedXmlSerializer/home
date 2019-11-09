using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Content
{
	[UsedImplicitly]
	sealed class VariableTypeElement : IElement
	{
		readonly IIdentities    _identities;
		readonly RuntimeElement _runtime;

		public VariableTypeElement(IIdentities identities, RuntimeElement runtime)
		{
			_identities = identities;
			_runtime    = runtime;
		}

		public IWriter Get(TypeInfo parameter)
			=> new VariableTypeIdentity(parameter.AsType(), _identities.Get(parameter), _runtime);
	}
}