using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	public sealed class DefaultReferencesExtension : ISerializerExtension
	{
		public DefaultReferencesExtension() : this(new HashSet<TypeInfo> {Support<string>.Metadata}) {}

		public DefaultReferencesExtension(ICollection<TypeInfo> blacklist) : this(blacklist, new HashSet<TypeInfo>()) {}

		public DefaultReferencesExtension(ICollection<TypeInfo> blacklist, ICollection<TypeInfo> whitelist)
		{
			Blacklist = blacklist;
			Whitelist = whitelist;
		}

		public ICollection<TypeInfo> Blacklist { get; }
		public ICollection<TypeInfo> Whitelist { get; }

		public IServiceRepository Get(IServiceRepository parameter)
		{
			var policy = Whitelist.Any()
				             ? (IReferencesPolicy)new WhitelistReferencesPolicy(Whitelist.ToArray())
				             : new BlacklistReferencesPolicy(Blacklist.ToArray());

			return parameter.RegisterInstance(policy)
			                .Register<ContainsStaticReferenceSpecification>()
			                .Register<IStaticReferenceSpecification, ContainsStaticReferenceSpecification>()
			                .Register<IReferences, References>()
				/*.Decorate(typeof(IContentWriters<>), typeof(ContentWriters<>))*/;
		}

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}