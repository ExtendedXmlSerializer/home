using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ExtensionModel.Xml.Classic
{
	sealed class TypeIdentityRegistrations : ITypeIdentityRegistrations
	{
		readonly Func<TypeInfo, IEnumerable<TypeInfo>> _types;
		readonly ITypeIdentity                         _identity;
		readonly IIdentityStore                        _store;
		readonly INamespaceFormatter                   _formatter;

		public TypeIdentityRegistrations(IDiscoveredTypes types, ITypeIdentity identity, IIdentityStore store,
		                                 INamespaceFormatter formatter)
			: this(types.To(EnumerableCoercer<TypeInfo>.Default)
			            .ToSelectionDelegate(), identity, store, formatter) {}

		TypeIdentityRegistrations(Func<TypeInfo, IEnumerable<TypeInfo>> types, ITypeIdentity identity,
		                          IIdentityStore store,
		                          INamespaceFormatter formatter)
		{
			_types     = types;
			_identity  = identity;
			_store     = store;
			_formatter = formatter;
		}

		public IEnumerable<KeyValuePair<TypeInfo, IIdentity>> Get(IEnumerable<TypeInfo> parameter)
		{
			foreach (var type in parameter.SelectMany(_types)
			                              .Distinct())
			{
				var key = _identity.Get(type);
				if (key != null)
				{
					var identifier = key.Value.Identifier.NullIfEmpty() ?? _formatter.Get(type);
					var name       = key.Value.Name.NullIfEmpty() ?? type.Name;
					var identity   = _store.Get(name, identifier);
					yield return Pairs.Create(type, identity);
				}
			}
		}
	}
}