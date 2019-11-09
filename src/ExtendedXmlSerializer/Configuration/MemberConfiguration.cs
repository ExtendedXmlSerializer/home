using ExtendedXmlSerializer.ExtensionModel.Content.Members;
using System.Reflection;

// ReSharper disable UnusedTypeParameter

namespace ExtendedXmlSerializer.Configuration
{
	class MemberConfiguration<T, TMember>
		: TypeConfiguration<T>, IMemberConfiguration<T, TMember>, IInternalMemberConfiguration
	{
		readonly IProperty<string> _name;
		readonly IProperty<int>    _order;
		readonly MemberInfo        _member;

		public MemberConfiguration(ITypeConfigurationContext parent, MemberInfo member)
			: this(parent, parent.Root.With<MemberPropertiesExtension>(), member) {}

		MemberConfiguration(ITypeConfigurationContext parent, MemberPropertiesExtension extension,
		                    MemberInfo member) : this(parent, new MemberProperty<string>(extension.Names, member),
		                                              new MemberProperty<int>(extension.Order, member),
		                                              member) {}

		// ReSharper disable once TooManyDependencies
		public MemberConfiguration(ITypeConfigurationContext parent, IProperty<string> name, IProperty<int> order,
		                           MemberInfo member) : base(parent, name)
		{
			_name   = name;
			_order  = order;
			_member = member;
		}

		IMemberConfiguration IInternalMemberConfiguration.Name(string name)
		{
			_name.Assign(name);
			return this;
		}

		IMemberConfiguration IInternalMemberConfiguration.Order(int order)
		{
			_order.Assign(order);
			return this;
		}

		public new MemberInfo Get() => _member;
	}
}