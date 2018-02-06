using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using JetBrains.Annotations;

namespace ExtendedXmlSerializer.ContentModel.Content {
	[UsedImplicitly]
	sealed class VariableTypeElement : IElements
	{
		readonly IIdentities _identities;

		public VariableTypeElement(IIdentities identities) => _identities = identities;

		public IWriter Get(TypeInfo parameter)
			=> new VariableTypeIdentity<object>(parameter.AsType(), _identities.Get(parameter), _identities).Adapt();
	}
}