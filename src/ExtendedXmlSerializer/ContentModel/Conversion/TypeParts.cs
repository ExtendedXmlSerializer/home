using ExtendedXmlSerializer.ContentModel.Identification;
using System;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ContentModel.Conversion
{
	struct TypeParts : IIdentity
	{
		readonly Func<ImmutableArray<TypeParts>> _arguments;

		// ReSharper disable once TooManyDependencies
		public TypeParts(string name, string identifier = "", Func<ImmutableArray<TypeParts>> arguments = null,
		                 ImmutableArray<int>? dimensions = null)
		{
			Name       = name;
			Identifier = identifier;
			Dimensions = dimensions;
			_arguments = arguments;
		}

		public string Identifier { get; }
		public ImmutableArray<int>? Dimensions { get; }
		public string Name { get; }

		public ImmutableArray<TypeParts>? GetArguments() => _arguments?.Invoke();
	}
}