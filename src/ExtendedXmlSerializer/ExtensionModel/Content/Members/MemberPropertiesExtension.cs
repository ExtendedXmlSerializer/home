using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class MemberPropertiesExtension : ISerializerExtension
	{
		readonly INames                                _defaultNames;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		public MemberPropertiesExtension(INames defaultNames, IParameterizedSource<MemberInfo, int> defaultMemberOrder)
			: this(new Dictionary<MemberInfo, string>(), new Dictionary<MemberInfo, int>(), defaultNames,
			       defaultMemberOrder) {}

		// ReSharper disable once TooManyDependencies
		public MemberPropertiesExtension(IDictionary<MemberInfo, string> names, IDictionary<MemberInfo, int> order,
		                                 INames defaultNames, IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_defaultNames       = defaultNames;
			_defaultMemberOrder = defaultMemberOrder;
			Order               = order;
			Names               = names;
		}

		public IDictionary<MemberInfo, string> Names { get; }
		public IDictionary<MemberInfo, int> Order { get; }

		public IServiceRepository Get(IServiceRepository parameter) =>
			parameter
				.RegisterInstance<INames>(new MemberNames(new MemberTable<string>(Names).Or(_defaultNames)))
				.RegisterInstance<IMemberOrder>(new MemberOrder(Order, _defaultMemberOrder));

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}