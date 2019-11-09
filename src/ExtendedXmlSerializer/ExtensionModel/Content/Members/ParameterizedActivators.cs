using ExtendedXmlSerializer.ContentModel.Members;
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
	sealed class ParameterizedActivators : IActivators
	{
		readonly IActivators          _activators;
		readonly IQueriedConstructors _constructors;
		readonly IMemberAccessors     _accessors;
		readonly IConstructorMembers  _members;

		// ReSharper disable once TooManyDependencies
		public ParameterizedActivators(IActivators activators, IQueriedConstructors constructors,
		                               IConstructorMembers members, IMemberAccessors accessors)
		{
			_activators   = activators;
			_constructors = constructors;
			_members      = members;
			_accessors    = accessors;
		}

		public IActivator Get(Type parameter)
		{
			var typeInfo    = parameter.GetTypeInfo();
			var constructor = _constructors.Get(typeInfo);
			var members     = constructor != null ? _members.Get(constructor) : null;
			var result = members != null
				             ? Activator(constructor, members.GetValueOrDefault())
				             : _activators.Get(typeInfo);
			return result;
		}

		ActivationContextActivator Activator(ConstructorInfo constructor, ImmutableArray<IMember> members)
		{
			var activator = new Source(constructor).ToDelegate();
			var contexts  = new ActivationContexts(_accessors, members, activator);
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