using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	public sealed class AllowedMembersExtension : ISerializerExtension
	{
		readonly static Collection<MemberInfo> DefaultBlacklist =
			new Collection<MemberInfo>
			{
				typeof(IDictionary<,>).GetRuntimeProperty(nameof(IDictionary.Keys)),
				typeof(IDictionary<,>).GetRuntimeProperty(nameof(IDictionary.Values))
			};

		readonly IMetadataSpecification _specification;

		public AllowedMembersExtension(IMetadataSpecification specification)
			: this(specification, new HashSet<MemberInfo>(DefaultBlacklist), new HashSet<MemberInfo>()) {}

		public AllowedMembersExtension(IMetadataSpecification specification, ICollection<MemberInfo> blacklist,
		                               ICollection<MemberInfo> whitelist)
		{
			_specification = specification;
			Blacklist      = blacklist;
			Whitelist      = whitelist;
		}

		public ICollection<MemberInfo> Blacklist { get; }
		public ICollection<MemberInfo> Whitelist { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var policy = Whitelist.Any()
				             ? (ISpecification<MemberInfo>)new WhitelistMemberPolicy(Whitelist.ToArray())
				             : new BlacklistMemberPolicy(Blacklist.ToArray());

			return parameter
			       .RegisterInstance(policy.And<PropertyInfo>(_specification))
			       .RegisterInstance(policy.And<FieldInfo>(_specification))
			       .Register<IMetadataSpecification, MetadataSpecification>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}