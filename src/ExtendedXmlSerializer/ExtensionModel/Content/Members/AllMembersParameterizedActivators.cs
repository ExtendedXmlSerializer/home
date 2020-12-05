using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	sealed class AllMembersParameterizedActivators : IActivators
	{
		readonly IActivators          _activators;
		readonly IQueriedConstructors _constructors;
		readonly IMemberAccessors     _accessors;
		readonly ITypeMembers         _typeMembers;
		readonly ITypeDefaults        _defaults;
		readonly IConstructorMembers  _members;

		// ReSharper disable once TooManyDependencies
		public AllMembersParameterizedActivators(IActivators activators, IQueriedConstructors constructors,
		                                         IConstructorMembers members, IMemberAccessors accessors,
		                                         ITypeMembers typeMembers)
			: this(activators, constructors, members, accessors, typeMembers, TypeDefaults.Defaults.Get(activators)) {}

		// ReSharper disable once TooManyDependencies
		public AllMembersParameterizedActivators(IActivators activators, IQueriedConstructors constructors,
		                                         IConstructorMembers members, IMemberAccessors accessors,
		                                         ITypeMembers typeMembers, ITypeDefaults defaults)
		{
			_activators   = activators;
			_constructors = constructors;
			_members      = members;
			_accessors    = accessors;
			_typeMembers  = typeMembers;
			_defaults     = defaults;
		}

		public IActivator Get(Type parameter)
		{
			var typeInfo    = parameter.GetTypeInfo();
			var constructor = _constructors.Get(typeInfo);
			var members     = constructor != null ? _members.Get(constructor) : null;

			var result = members != null
				             ? Activator(constructor, _typeMembers.Get(typeInfo))
				             : _activators.Get(typeInfo);
			return result;
		}

		ActivationContextActivator Activator(ConstructorInfo constructor, ImmutableArray<IMember> members)
		{
			var activator = new Source(constructor).ToSelectionDelegate();
			var context   = new MemberContext(constructor.ReflectedType.GetTypeInfo(), members);
			var contexts  = new ActivationContexts(_accessors, context, activator, _defaults);
			var defaults = constructor.GetParameters()
			                          .Where(x => x.IsOptional)
			                          .Select(x => Pairs.Create(x.Name, x.DefaultValue))
			                          .ToImmutableArray();
			var result = new ActivationContextActivator(contexts, defaults);
			return result;
		}

		sealed class Source : IParameterizedSource<Func<string, object>, IActivator>
		{
			readonly ConstructorInfo _constructor;

			public Source(ConstructorInfo constructor) => _constructor = constructor;

			public IActivator Get(Func<string, object> parameter)
			{
				var arguments = new Enumerable<object>(_constructor.GetParameters()
				                                                   .Select(x => x.Name)
				                                                   .Select(parameter));
				var result = new ConstructedActivator(_constructor, arguments);
				return result;
			}
		}
	}
}