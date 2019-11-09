using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	class Activated<T> : Generic<object, T>, IParameterizedSource<IServices, T>, IAlteration<IServiceRepository>
		where T : class
	{
		readonly ISingletonLocator _locator;
		readonly Type              _objectType;
		readonly TypeInfo          _targetType;

		public Activated(Type objectType, TypeInfo targetType, Type definition) : this(SingletonLocator.Default,
		                                                                               objectType,
		                                                                               targetType, definition) {}

		// ReSharper disable once TooManyDependencies
		public Activated(ISingletonLocator locator, Type objectType, TypeInfo targetType, Type definition)
			: base(definition)
		{
			_locator    = locator;
			_objectType = objectType;
			_targetType = targetType;
		}

		public T Get(IServices parameter)
		{
			var service = parameter.GetService(_objectType);

			var result = service as T ?? Get(_targetType.Yield()
			                                            .ToImmutableArray())
				             .Invoke(service);
			return result;
		}

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var singleton = _locator.Get(_objectType);
			var result = singleton != null
				             ? parameter.RegisterInstance(_objectType, singleton)
				             : parameter.Register(_objectType);
			return result;
		}
	}
}