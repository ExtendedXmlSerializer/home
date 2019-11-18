using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// Default serializer extension that is used to configure member properties such as name and ordering.
	/// </summary>
	public sealed class MemberPropertiesExtension : ISerializerExtension
	{
		readonly INames                                _defaultNames;
		readonly IParameterizedSource<MemberInfo, int> _defaultMemberOrder;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.Members.MemberPropertiesExtension"/> class.
		/// </summary>
		/// <param name="defaultNames">The default names.</param>
		/// <param name="defaultMemberOrder">The default member order.</param>
		public MemberPropertiesExtension(INames defaultNames, IParameterizedSource<MemberInfo, int> defaultMemberOrder)
			: this(new Dictionary<MemberInfo, string>(), new Dictionary<MemberInfo, int>(), defaultNames,
			       defaultMemberOrder) {}

		// ReSharper disable once TooManyDependencies
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExtendedXmlSerializer.ExtensionModel.Content.Members.MemberPropertiesExtension"/> class.
		/// </summary>
		/// <param name="names">The names.</param>
		/// <param name="order">The order.</param>
		/// <param name="defaultNames">The default names.</param>
		/// <param name="defaultMemberOrder">The default member order.</param>
		public MemberPropertiesExtension(IDictionary<MemberInfo, string> names, IDictionary<MemberInfo, int> order,
		                                 INames defaultNames, IParameterizedSource<MemberInfo, int> defaultMemberOrder)
		{
			_defaultNames       = defaultNames;
			_defaultMemberOrder = defaultMemberOrder;
			Order               = order;
			Names               = names;
		}

		/// <summary>
		/// A registry of text names, keyed by member metadata.
		/// </summary>
		public IDictionary<MemberInfo, string> Names { get; }

		/// <summary>
		/// A registry of member order values, keyed on member metadata.
		/// </summary>
		public IDictionary<MemberInfo, int> Order { get; }

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<INames>(new MemberNames(new MemberTable<string>(Names).Or(_defaultNames)))
			            .RegisterInstance<IMemberOrder>(new MemberOrder(Order, _defaultMemberOrder));

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}