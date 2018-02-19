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

using ExtendedXmlSerializer.Core.Sources;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.Configuration
{
	/*public interface IServiceConfiguration<T> : IConfigurationElement, IEntry<T> {}*/

	/*sealed class TypeConfigurationMembers<T, TMember> : ReferenceCacheBase<IMember, MemberConfiguration<T, TMember>>
	{
		public static IParameterizedSource<ITypeConfiguration, TypeConfigurationMembers<T, TMember>> Defaults { get; }
			= new ReferenceCache<ITypeConfiguration, TypeConfigurationMembers<T, TMember>>(x => new TypeConfigurationMembers<T, TMember>(x));

		readonly ITypeConfiguration _element;

		public TypeConfigurationMembers(ITypeConfiguration element) => _element = element;

		protected override MemberConfiguration<T, TMember> Create(IMember parameter)
			=> new MemberConfiguration<T, TMember>(_element, parameter);
	}*/


	public class TypeConfiguration<T> : MetadataConfiguration<TypeMetadata>, IType<T>
	{
		readonly IValueSource<MemberInfo, IMetadata> _members;

		[UsedImplicitly]
		public TypeConfiguration(IExtensions extensions) : this(extensions, new TypeMetadata<T>()) {}

		public TypeConfiguration(IExtensions extensions, TypeMetadata metadata)
			: this(extensions, metadata, new TableValueSource<MemberInfo, IMetadata>(new MemberInstances(metadata).Get)) {}

		public TypeConfiguration(IExtensions extensions, TypeMetadata metadata, IValueSource<MemberInfo, IMetadata> members)
			: base(extensions, metadata) => _members = members;

		public IMetadata Get(MemberInfo parameter) => _members.Get(parameter);

		public IEnumerator<IMetadata> GetEnumerator() => _members.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/*public class TypeConfiguration<T> : TypeConfigurationElement<T>
	{
		readonly IEntries _entries;
		readonly IMemberConfigurations _members;

		public TypeConfiguration(IConfigurationElement element, IEntries entries, IMemberConfigurations members) : base(element, members)
		{
			_entries = entries;
			_members  = members;
		}

		public TypeConfiguration<T, TService> Registration<TService>(IRegistrations<TService> registrations)
			=> new TypeConfiguration<T, TService>(this, _entries, _members, _entries.To(A<IEntry<TService>>.Default)
			                                                                                 .Get(registrations.Get()));
	}

	public class TypeConfiguration<T, TService> : TypeConfiguration<T>, IServiceConfiguration<TService>
	{
		readonly IEntry<TService> _entry;

		public TypeConfiguration(IConfigurationElement element, IEntries entries,
		                                IMemberConfigurations members, IEntry<TService> entry)
			: base(element, entries, members) => _entry = entry;

		public TService Get() => _entry.Get();

		public void Execute(TService parameter) => _entry.Execute(parameter);

		public void Execute(Unit parameter) => _entry.Execute(parameter);
	}

	public interface IEntry<T> : ISource<T>, ICommand<T>, ICommand<Unit> {}

	class Entry<T> : DelegatedSource<T>, IEntry<T>
	{
		readonly Action<T> _assign;
		readonly Action _clear;

		public Entry(Func<T> get, Action<T> assign, Action clear) : base(get)
		{
			_assign = assign;
			_clear = clear;
		}

		public void Execute(T parameter)
		{
			_assign(parameter);
		}

		public void Execute(Unit parameter)
		{
			_clear();
		}
	}

	public struct Registration<T>
	{
		public Registration(IRegistrations<T> source, T instance)
		{
			Source = source;
			Instance = instance;
		}

		public IRegistrations<T> Source { get; }
		public T Instance { get; }
	}

	public interface IRegistrations<T> : ISource<TypeInfo>, IAlteration<T> {}

	sealed class ContentSerializerRegistrations<T> : ServiceRegistrations<IContentSerializer<T>>
	{
		public static ContentSerializerRegistrations<T> Default { get; } = new ContentSerializerRegistrations<T>();
		ContentSerializerRegistrations() : base(new GenericRegistrations<IContentSerializer<T>>()) {}
	}

	class GenericRegistrations<T> : Registrations<T>
	{
		public GenericRegistrations() : base(Support<T>.Definition.GetTypeInfo()) {}
	}

	public interface IServiceRegistrations<T> : IRegistrations<IService<T>>,
	                                            IParameterizedSource<Type, Registration<IService<T>>>,
	                                            IParameterizedSource<T, Registration<IService<T>>> {}


	class ServiceRegistrations<T> : IServiceRegistrations<T>
	{
		readonly IRegistrations<T> _registrations;

		public ServiceRegistrations(IRegistrations<T> registrations) => _registrations = registrations;

		public TypeInfo Get() => _registrations.Get();

		public IService<T> Get(IService<T> parameter) => parameter;

		public Registration<IService<T>> Get(Type parameter) => new Registration<IService<T>>(this, new Service<T>(parameter));

		public Registration<IService<T>> Get(T parameter) => new Registration<IService<T>>(this, new InstanceService<T>(parameter));
	}

	class Registrations<T> : FixedInstanceSource<TypeInfo>, IRegistrations<T>
	{
		public Registrations(TypeInfo instance) : base(instance) {}

		public T Get(T parameter) => parameter;
	}*/

	/*public class ServiceConfiguration<T, TService> : TypeConfiguration<T>, IMembership<TService>, ICommand<TService>
	{
		public ServiceConfiguration(IConfigurationElement element, IParameterizedSource<object, IMembership<object>> services,
		                            IMemberConfigurations members, ICommand<TService> add, ICommand<TService> remove)
			: base(element, services, members)
		{
			Add = add;
			Remove = remove;
		}

		public ICommand<TService> Add { get; }

		public ICommand<TService> Remove { get; }

		public void Execute(TService parameter) => Add.Execute(parameter);
	}

	public interface IServiceRegistration<out T> : IService<T>, ISource<object> {}

	class ServiceRegistration<T> : FixedInstanceSource<object>, IServiceRegistration<T>
	{
		readonly IService<T> _service;

		public ServiceRegistration(object key, Type serviceType) : this(key, new Service<T>(serviceType)) {}

		public ServiceRegistration(object key, T instance) : this(key, new InstanceService<T>(instance)) {}

		public ServiceRegistration(object key, IService<T> service) : base(key) => _service = service;

		public T Get(IServiceProvider parameter) => _service.Get(parameter);
	}

	class GenericServiceRegistration<T> : ServiceRegistration<T>
	{
		public GenericServiceRegistration(Type serviceType) : base(Support<T>.Definition, serviceType) {}
		public GenericServiceRegistration(T instance) : base(Support<T>.Definition, instance) {}
		public GenericServiceRegistration(IService<T> service) : base(Support<T>.Definition, service) {}
	}

	sealed class ContentSerializerRegistration<T> : GenericServiceRegistration<IContentSerializer<T>>
	{
		public ContentSerializerRegistration(Type serviceType) : base(serviceType) {}
		public ContentSerializerRegistration(IContentSerializer<T> instance) : base(instance) {}
		public ContentSerializerRegistration(IService<IContentSerializer<T>> service) : base(service) {}
	}*/
}