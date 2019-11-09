using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class ObjectIdentifiers : IObjectIdentifiers
	{
		readonly ITypeMembers           _members;
		readonly IEnumeratorStore       _enumerators;
		readonly IMemberAccessors       _accessors;
		readonly Func<TypeInfo, string> _names;

		[UsedImplicitly]
		public ObjectIdentifiers(IIdentifiers identifiers, ITypeMembers members, IEnumeratorStore enumerators,
		                         IMemberAccessors accessors)
			: this(members, enumerators, accessors, identifiers.Get) {}

		ObjectIdentifiers(ITypeMembers members, IEnumeratorStore enumerators, IMemberAccessors accessors,
		                  Func<TypeInfo, string> names)
		{
			_members     = members;
			_enumerators = enumerators;
			_accessors   = accessors;
			_names       = names;
		}

		public ImmutableArray<string> Get(object parameter)
			=> new ObjectTypeWalker(_members, _enumerators, _accessors, parameter).Get()
			                                                                      .Select(_names)
			                                                                      .Distinct()
			                                                                      .Skip(1)
			                                                                      .ToImmutableArray();
	}
}