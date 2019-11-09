using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	/*public interface ITypeMemberConfigurations : IParameterizedSource<ITypeConfiguration, IReadOnlyDictionary<MemberInfo, IMemberConfiguration>> {}

	sealed class TypeMemberConfigurations : ITypeMemberConfigurations
	{
		public IReadOnlyDictionary<MemberInfo, IMemberConfiguration> Get(ITypeConfiguration parameter)
		{
			throw new System.NotImplementedException();
		}

		sealed class Source : Generic<IContext, ContentModel.Properties.IProperty<string>, ContentModel.Properties.IProperty<int>, IMemberConfiguration>
		{
			public static Source Default { get; } = new Source();
			Source() : base(typeof(MemberConfiguration<,>)) { }
		}
	}*/

	/*public interface IMemberConfigurations : IParameterizedSource<MemberInfo, IMemberConfiguration> {}*/

	// ReSharper disable once UnusedTypeParameter
	sealed class MemberConfigurations<T> : CacheBase<MemberInfo, IMemberConfiguration>, IMemberConfigurations
	{
		readonly ITypeConfigurationContext                              _parent;
		readonly ConcurrentDictionary<MemberInfo, IMemberConfiguration> _store;

		public MemberConfigurations(ITypeConfigurationContext parent) :
			this(parent, new ConcurrentDictionary<MemberInfo, IMemberConfiguration>()) {}

		public MemberConfigurations(ITypeConfigurationContext parent,
		                            ConcurrentDictionary<MemberInfo, IMemberConfiguration> store)
			: base(store)
		{
			_parent = parent;
			_store  = store;
		}

		protected override IMemberConfiguration Create(MemberInfo parameter)
			=> Source.Default
			         .Get(_parent.Get(), MemberDescriptors.Default.Get(parameter)
			                                              .MemberType)
			         .Invoke(_parent, parameter);

		sealed class Source : Generic<ITypeConfigurationContext, MemberInfo, IMemberConfiguration>
		{
			public static Source Default { get; } = new Source();

			Source() : base(typeof(MemberConfiguration<,>)) {}
		}

		public IEnumerator<IMemberConfiguration> GetEnumerator() => _store.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

/*
	sealed class Property<T> : IProperty<T>
	{
		T _value;

		public T Get() => _value;

		public void Assign(T value) => _value = value;
	}
*/
}