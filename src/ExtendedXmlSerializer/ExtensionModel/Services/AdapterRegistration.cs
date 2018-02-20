// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Collections;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Services
{
	sealed class Registration<TFrom, TTo> : ImplementationRegistration where TTo : class, TFrom
	{
		public static Registration<TFrom, TTo> Default { get; } = new Registration<TFrom, TTo>();
		Registration() : base(typeof(TFrom), typeof(TTo)) {}
	}

	sealed class RegisterWithDependencies<TFrom, TTo> : CompositeRegistration where TTo : class, TFrom
	{
		public static RegisterWithDependencies<TFrom, TTo> Default { get; } = new RegisterWithDependencies<TFrom, TTo>();
		RegisterWithDependencies() : base(new ImplementationRegistration(typeof(TFrom), typeof(TTo)), RegisterDependencies<TTo>.Default) { }
	}

	sealed class RegisterDependencies<T> : DecoratedRegistration
	{
		public static RegisterDependencies<T> Default { get; } = new RegisterDependencies<T>();
		RegisterDependencies() : base(new RegisterDependencies(typeof(T))) {}
	}

	sealed class RegisterDependencies : IRegistration
	{
		readonly IConstructors _constructors;
		readonly Type _type;

		public RegisterDependencies(Type type) : this(Constructors.Default, type) {}

		public RegisterDependencies(IConstructors constructors, Type type)
		{
			_constructors = constructors;
			_type = type;
		}

		public IServiceRepository Get(IServiceRepository parameter)
			=> _constructors.Get(_type)
			                .SelectMany(x => x.GetParameters()
			                                  .Select(y => y.ParameterType)
			                                  .Select(y => _type.IsGenericTypeDefinition && y.IsConstructedGenericType &&
			                                               !y.IsGenericTypeDefinition
				                                               ? y.GetGenericTypeDefinition()
				                                               : y))
			                .Where(x => x.IsClass && !parameter.AvailableServices.Contains(x))
			                .Aggregate(parameter, (repository, t) => repository.Register(t)
			                                                                   .RegisterDependencies(t));
	}

	class ImplementationRegistration : IRegistration
	{
		readonly Type _from;
		readonly Type _to;

		public ImplementationRegistration(Type @from, Type @to)
		{
			_from = @from;
			_to = to;
		}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Register(_from, _to);
	}

	class Registration : IRegistration
	{
		readonly Type _type;

		public Registration(Type type) => _type = type;

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Register(_type);
	}

	sealed class Registration<T> : ImplementationRegistration
	{
		public Registration(Type type) : base(typeof(T), type) {}
	}

	class CompositeRegistration : IRegistration
	{
		readonly ImmutableArray<IRegistration> _configurations;
		public CompositeRegistration(params IRegistration[] configurations) : this(configurations.AsEnumerable()) {}
		public CompositeRegistration(IEnumerable<IRegistration> registrations) : this(registrations.ToImmutableArray()) {}
		public CompositeRegistration(ImmutableArray<IRegistration> configurations) => _configurations = configurations;

		public IServiceRepository Get(IServiceRepository parameter) => _configurations.ToArray().Alter(parameter);
	}

	public interface IService<out T> : IParameterizedSource<System.IServiceProvider, T> {}

	public interface IServiceGroupCollection<T> : IGroupCollection<IService<T>> {}

	public class ServiceGroupCollection<T> : GroupCollection<IService<T>>, IRegistration
	{
		public ServiceGroupCollection(IEnumerable<IGroup<IService<T>>> groups) : base(groups) {}

		public IServiceRepository Get(IServiceRepository parameter) => this.OfType<IRegistration>()
		                                                                   .ToArray()
		                                                                   .Alter(parameter);
	}

	// ReSharper disable once PossibleInfiniteInheritance
	class ServiceGroups<T> : Groups<IService<T>>
	{
		public ServiceGroups(IEnumerable<GroupName> phases) : base(phases) { }
	}

	sealed class ServicePropertyReference<TProperty, T> : PropertyReference<TProperty, IService<T>>
		where TProperty : class, IProperty<IService<T>>
	{
		public ServicePropertyReference(IMetadataTable table, ISingletonLocator locator) : base(table, locator) {}
	}

	class ServiceProperty<TProperty, T> : SpecificationSource<MemberInfo, T> where TProperty : class, IServiceProperty<T>
	{
		public ServiceProperty(IServiceCoercer<T> coercer, ServicePropertyReference<TProperty, T> property)
			: base(property, property.Out(coercer)) {}
	}

	public interface IServiceProperty<T> : IProperty<IService<T>> {}

	class ServiceProperty<T> : Property<IService<T>>, IServiceProperty<T> {}

	public sealed class ExtensionServicesExtension : ISerializerExtension
	{
		public static ExtensionServicesExtension Default { get; } = new ExtensionServicesExtension();
		ExtensionServicesExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterDefinition<IServiceCoercer<object>, ServiceCoercer<object>>();

		public void Execute(IServices parameter) {}
	}

	public interface IServiceCoercer<T> :  IParameterizedSource<IService<T>, T> {}

	sealed class ServiceCoercer<T> : IServiceCoercer<T>
	{
		readonly System.IServiceProvider _provider;

		public ServiceCoercer(System.IServiceProvider provider) => _provider = provider;

		public T Get(IService<T> parameter) => parameter.Get(_provider);
	}

	class Service<T> : DecoratedRegistration, IService<T>
	{
		readonly Type _type;

		public Service(Type type) : base(new Registration<T>(type)) => _type = type;

		public T Get(System.IServiceProvider parameter) => parameter.Get<T>(_type);
	}

	class InstanceService<T> : FixedInstanceSource<System.IServiceProvider, T>, IService<T>
	{
		public InstanceService(T instance) : base(instance) {}
	}

	class RegisteredService<T> : IRegisteredService<T>
	{
		readonly IParameterizedSource<System.IServiceProvider, T> _service;
		readonly IAlteration<IServiceRepository> _registration;

		public RegisteredService(IParameterizedSource<System.IServiceProvider, T> service) : this(service, Self<IServiceRepository>.Default) {}

		public RegisteredService(IParameterizedSource<System.IServiceProvider, T> service, IAlteration<IServiceRepository> registration)
		{
			_service = service;
			_registration = registration;
		}

		public T Get(System.IServiceProvider parameter) => _service.Get(parameter);

		public IServiceRepository Get(IServiceRepository parameter) => _registration.Get(parameter);
	}

	sealed class AdapterLocator<T, TDefinition> : AdapterLocator<T>
	{
		public AdapterLocator(Type objectType, TypeInfo argumentType)
			: base(typeof(TDefinition).GetGenericTypeDefinition(), objectType, argumentType) {}
	}

	class AdapterLocator<T> : IParameterizedSource<System.IServiceProvider, T>
	{
		readonly IGeneric<object, T> _generic;
		readonly Type _objectType;
		readonly TypeInfo _targetType;

		public AdapterLocator(Type definition, Type objectType, TypeInfo argumentType)
			: this(new Generic<object, T>(definition), objectType, argumentType) {}

		public AdapterLocator(IGeneric<object, T> generic, Type objectType, TypeInfo targetType)
		{
			_generic = generic;
			_objectType = objectType;
			_targetType = targetType;
		}

		public T Get(System.IServiceProvider parameter)
		{
			var service = parameter.GetService(_objectType);
			var result = service is T cast
				             ? cast
				             : _generic.Get(_targetType)
				                       .Invoke(service);
			return result;
		}
	}

	sealed class AdapterRegistration : IRegistration
	{
		readonly ISingletonLocator _locator;
		readonly Type _objectType;

		public AdapterRegistration(Type objectType) : this(SingletonLocator.Default, objectType) {}

		public AdapterRegistration(ISingletonLocator locator, Type objectType)
		{
			_locator = locator;
			_objectType = objectType;
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