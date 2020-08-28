using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class References : IReferences
	{
		readonly ISpecification<TypeInfo> _allow;
		readonly IReferencesPolicy        _policy;
		readonly ITypeMembers             _members;
		readonly IEnumeratorStore         _enumerators;
		readonly IMemberAccessors         _accessors;

		// ReSharper disable once TooManyDependencies
		public References(IContainsCustomSerialization custom, IReferencesPolicy policy, ITypeMembers members,
		                  IEnumeratorStore enumerators, IMemberAccessors accessors)
			: this(AssignedSpecification<TypeInfo>.Default.And(custom.Inverse()), policy, members, enumerators,
			       accessors) {}

		// ReSharper disable once TooManyDependencies
		public References(ISpecification<TypeInfo> allow, IReferencesPolicy policy, ITypeMembers members,
		                  IEnumeratorStore enumerators, IMemberAccessors accessors)
		{
			_allow       = allow;
			_policy      = policy;
			_members     = members;
			_enumerators = enumerators;
			_accessors   = accessors;
		}

		public ImmutableArray<object> Get(object parameter)
			=> new ReferenceWalker(_allow, _policy, _members, _enumerators, _accessors, parameter).Get();

	}
}