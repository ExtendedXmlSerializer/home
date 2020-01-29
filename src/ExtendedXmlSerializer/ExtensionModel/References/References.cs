using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class References : StructureCacheBase<object, ImmutableArray<object>>, IReferences
	{
		readonly IReferencesPolicy _policy;
		readonly ITypeMembers      _members;
		readonly IEnumeratorStore  _enumerators;
		readonly IMemberAccessors  _accessors;
		readonly ISpecification<TypeInfo> _contains;

		// ReSharper disable once TooManyDependencies
		public References(IReferencesPolicy policy, ITypeMembers members, IEnumeratorStore enumerators,
		                  IMemberAccessors accessors, IContainsCustomSerialization custom)
		{			
			_policy      = policy;
			_members     = members;
			_enumerators = enumerators;
			_accessors   = accessors;
			_contains = custom;
		}

		protected override ImmutableArray<object> Create(object parameter)
			=> new ReferenceWalker(AssignedSpecification<TypeInfo>.Default.And(_contains.Inverse()), _policy, _members, _enumerators, _accessors, parameter).Get();
	}
}