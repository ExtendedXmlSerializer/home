using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class References : IReferences
	{
		readonly ReferenceWalker _walker;

		// ReSharper disable once TooManyDependencies
		public References(IContainsCustomSerialization custom, IReferencesPolicy policy, ITypeMembers members,
		                  IEnumeratorStore enumerators, IMemberAccessors accessors)
			: this(new ReferenceWalker(policy,
			                                    new IterateReferences(new ReferenceMembers(custom, members,
			                                                                               accessors, enumerators)))) {}

		// ReSharper disable once TooManyDependencies
		References(ReferenceWalker walker) => _walker = walker;

		public ImmutableArray<object> Get(object parameter)
		{
			using var lease  = _walker.Get(parameter);
			var       result = lease.Distinct().ToImmutableArray();
			return result;
		}
	}
}