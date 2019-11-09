using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Core
{
	sealed class ServiceProvider : ISpecification<Type>, IServiceProvider
	{
		readonly ImmutableArray<object>   _services;
		readonly ImmutableArray<TypeInfo> _types;

		public ServiceProvider(params object[] services) : this(services.ToImmutableArray()) {}

		public ServiceProvider(ImmutableArray<object> services)
			: this(services, services.Select(x => x.GetType()
			                                       .GetTypeInfo())
			                         .ToImmutableArray()) {}

		public ServiceProvider(ImmutableArray<object> services, ImmutableArray<TypeInfo> types)
		{
			_services = services;
			_types    = types;
		}

		public object GetService(Type serviceType)
		{
			var info   = serviceType.GetTypeInfo();
			var length = _services.Length;

			for (var i = 0; i < length; i++)
			{
				var item = _services[i];
				if (info.IsInstanceOfType(item))
				{
					return item;
				}
			}

			return null;
		}

		public bool IsSatisfiedBy(Type parameter)
			=> _types.Any(IsAssignableSpecification.Delegates.Get(parameter.GetTypeInfo()));
	}
}