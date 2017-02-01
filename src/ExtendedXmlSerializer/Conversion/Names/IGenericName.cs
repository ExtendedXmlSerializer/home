using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Names
{
	public interface IGenericName : IName
	{
		ImmutableArray<IName> Arguments { get; }
	}

	class GenericName : Name, IGenericName
	{
		public GenericName(string displayName, TypeInfo classification, ImmutableArray<IName> arguments)
			: base(displayName, classification)
		{
			Arguments = arguments;
		}

		public ImmutableArray<IName> Arguments { get; }
	}
}