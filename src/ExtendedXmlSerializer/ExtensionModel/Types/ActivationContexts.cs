using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ActivationContexts : IActivationContexts
	{
		readonly IMemberAccessors                       _accessors;
		readonly ImmutableArray<IMember>                _members;
		readonly ITableSource<string, IMember>          _table;
		readonly Func<Func<string, object>, IActivator> _activator;

		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members, IActivator activator)
			: this(accessors, members, activator.Accept) {}

		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members,
		                          Func<Func<string, object>, IActivator> activator)
			: this(accessors, members,
			       new TableSource<string, IMember>(members.ToDictionary(x => x.Name,
			                                                             StringComparer.InvariantCultureIgnoreCase)),
			       activator) {}

		// ReSharper disable once TooManyDependencies
		public ActivationContexts(IMemberAccessors accessors, ImmutableArray<IMember> members,
		                          ITableSource<string, IMember> table,
		                          Func<Func<string, object>, IActivator> activator)
		{
			_accessors = accessors;
			_members   = members;
			_table     = table;
			_activator = activator;
		}

		public IActivationContext Get(IDictionary<string, object> parameter)
		{
			var source = new TableSource<string, object>(parameter);
			var list   = new List<object>();
			var command = new CompositeCommand<object>(new ApplyMemberValuesCommand(_accessors, _members, source),
			                                           new AddItemsCommand(list));
			var alteration = new ConfiguringAlteration<object>(command);
			var activator  = new AlteringActivator(alteration, _activator(new Store(source, _table).Get));
			var result = new ActivationContext(source, activator.Singleton()
			                                                    .Get, list);
			return result;
		}

		sealed class Store : IParameterizedSource<string, object>
		{
			readonly ITableSource<string, object>          _store;
			readonly IParameterizedSource<string, IMember> _members;
			readonly ITypeDefaults                         _defaults;

			public Store(ITableSource<string, object> store, IParameterizedSource<string, IMember> members)
				: this(store, members, TypeDefaults.Default) {}

			public Store(ITableSource<string, object> store, IParameterizedSource<string, IMember> members,
			             ITypeDefaults defaults)
			{
				_store    = store;
				_members  = members;
				_defaults = defaults;
			}

			public object Get(string parameter)
				=> _store.IsSatisfiedBy(parameter) ? _store.Get(parameter) : Default(parameter);

			object Default(string parameter)
			{
				var member = _members.Get(parameter);
				var result = _defaults.Get(member.MemberType);
				return result;
			}
		}
	}
}