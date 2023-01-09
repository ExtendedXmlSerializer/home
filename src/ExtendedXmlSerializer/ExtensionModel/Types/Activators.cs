using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class Activators : ReferenceCacheBase<Type, IActivator>, IActivators
	{
		public Activators(IActivators activators)
			: this(ActivatingTypeSpecification.Default, activators, SingletonLocator.Default) {}

		readonly IActivatingTypeSpecification _specification;
		readonly IActivators                  _activators;
		readonly ISingletonLocator            _locator;

		public Activators(IActivatingTypeSpecification specification, IActivators activators, ISingletonLocator locator)
		{
			_specification = specification;
			_activators    = activators;
			_locator       = locator;
		}

		protected override IActivator Create(Type parameter)
		{
			var singleton = _locator.Get(parameter);
			var activator = _activators.Build(parameter);
			var activate  = _specification.IsSatisfiedBy(parameter);
			var result = singleton != null
				             ? activate
					               ? new Activator(_activators.Build(parameter), singleton)
					               : new ReflectionModel.Activator(singleton.Self)
				             : activator();
			return result;
		}

		sealed class Activator : IActivator
		{
			readonly Func<IActivator> _activator;
			readonly object           _singleton;

			public Activator(Func<IActivator> activator, object singleton)
			{
				_activator = activator;
				_singleton = singleton;
			}

			public object Get()
			{
				try
				{
					return _activator()
						.Get();
				}
				// ReSharper disable once CatchAllClause - In this case we fallback to singleton since it exists.
				catch
				{
					return _singleton;
				}
			}
		}
	}
}