using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class References : StructureCacheBase<object, ImmutableArray<object>>, IReferences
	{
		readonly IReferencesPolicy _policy;
		readonly ITypeMembers      _members;
		readonly IEnumeratorStore  _enumerators;
		readonly IMemberAccessors  _accessors;

		// ReSharper disable once TooManyDependencies
		public References(IReferencesPolicy policy, ITypeMembers members, IEnumeratorStore enumerators,
		                  IMemberAccessors accessors)
		{
			_policy      = policy;
			_members     = members;
			_enumerators = enumerators;
			_accessors   = accessors;
		}

		protected override ImmutableArray<object> Create(object parameter)
			=> new ReferenceWalker(_policy, _members, _enumerators, _accessors, parameter).Get();
	}
}