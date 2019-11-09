using System;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class IdentifierCommand : ICommand<IIdentityStore>
	{
		readonly IObjectIdentifiers _identifiers;
		readonly Func<bool>         _specification;
		readonly Func<object>       _source;

		public IdentifierCommand(IObjectIdentifiers identifiers, Func<bool> specification, Func<object> source)
		{
			_identifiers   = identifiers;
			_specification = specification;
			_source        = source;
		}

		public void Execute(IIdentityStore parameter)
		{
			if (_specification())
			{
				var identifiers = _identifiers.Get(_source());
				var length      = identifiers.Length;
				for (int i = 0; i < length; i++)
				{
					parameter.Get(string.Empty, identifiers[i]);
				}
			}
		}
	}
}