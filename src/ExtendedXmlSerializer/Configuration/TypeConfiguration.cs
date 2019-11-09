using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	class TypeConfiguration<T> : ConfigurationContainer, ITypeConfiguration<T>, IInternalTypeConfiguration
	{
		readonly IProperty<string>     _name;
		readonly IMemberConfigurations _members;

		public TypeConfiguration(IRootContext root, IProperty<string> name)
			: this(root, name, new MemberConfigurations<T>(new TypeConfigurationContext(root, Support<T>.Key))) {}

		public TypeConfiguration(IRootContext context, IProperty<string> name, IMemberConfigurations members)
			: base(context)
		{
			_name    = name;
			_members = members;
		}

		public TypeConfiguration(ITypeConfigurationContext context, IProperty<string> name,
		                         IMemberConfigurations members)
			: base(context)
		{
			_name    = name;
			_members = members;
		}

		public TypeConfiguration(ITypeConfigurationContext parent, IProperty<string> name)
			: this(parent, name,
			       new MemberConfigurations<T>(new TypeConfigurationContext(parent.Root, Support<T>.Key))) {}

		ITypeConfiguration IInternalTypeConfiguration.Name(string name)
		{
			_name.Assign(name);
			return this;
		}

		IMemberConfiguration IInternalTypeConfiguration.Member(MemberInfo member) => _members.Get(member);

		public new IEnumerator<IMemberConfiguration> GetEnumerator() => _members.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public TypeInfo Get() => Support<T>.Key;

		public IMemberConfiguration Get(MemberInfo parameter) => _members.Get(parameter);
	}
}