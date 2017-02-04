using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	public interface IGenericElement : IElement
	{
		ImmutableArray<IElement> Arguments { get; }
	}

	class GenericElement : ElementBase, IGenericElement
	{
		public GenericElement(string displayName, TypeInfo classification, ImmutableArray<IElement> arguments)
			: base(displayName, classification)
		{
			Arguments = arguments;
		}

		public ImmutableArray<IElement> Arguments { get; }
	}
}