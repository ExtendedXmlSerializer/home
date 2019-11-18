using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Content.Members
{
	/// <summary>
	/// A default extension that is used to determine which members are allowed to be considered for serialization and
	/// deserialization.
	/// </summary>
	public sealed class AllowedMembersExtension : ISerializerExtension
	{
		readonly static Collection<MemberInfo> DefaultBlacklist =
			new Collection<MemberInfo>
			{
				typeof(IDictionary<,>).GetRuntimeProperty(nameof(IDictionary.Keys)),
				typeof(IDictionary<,>).GetRuntimeProperty(nameof(IDictionary.Values))
			};

		readonly IMetadataSpecification _specification;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification"></param>
		public AllowedMembersExtension(IMetadataSpecification specification)
			: this(specification, new HashSet<MemberInfo>(DefaultBlacklist), new HashSet<MemberInfo>()) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="specification"></param>
		/// <param name="blacklist"></param>
		/// <param name="whitelist"></param>
		public AllowedMembersExtension(IMetadataSpecification specification, ICollection<MemberInfo> blacklist,
		                               ICollection<MemberInfo> whitelist)
		{
			_specification = specification;
			Blacklist      = blacklist;
			Whitelist      = whitelist;
		}

		/// <summary>
		/// List of prohibited members.
		/// </summary>
		public ICollection<MemberInfo> Blacklist { get; }

		/// <summary>
		/// List of allowed members.
		/// </summary>
		public ICollection<MemberInfo> Whitelist { get; }

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
		{
			var policy = Whitelist.Any()
				             ? (ISpecification<MemberInfo>)new WhitelistMemberPolicy(Whitelist.ToArray())
				             : new BlacklistMemberPolicy(Blacklist.ToArray());

			return parameter.RegisterInstance(policy.And<PropertyInfo>(_specification))
			                .RegisterInstance(policy.And<FieldInfo>(_specification))
			                .Register<IMetadataSpecification, MetadataSpecification>();
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}