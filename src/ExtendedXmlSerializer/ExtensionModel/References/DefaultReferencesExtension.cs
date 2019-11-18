using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	/// <summary>
	/// A default extension that provides basic support for circular reference detection, which by default throws when it
	/// is encountered.
	/// </summary>
	public sealed class DefaultReferencesExtension : ISerializerExtension
	{
		/// <summary>
		///  Creates a new instance with an empty blacklist.
		/// </summary>
		public DefaultReferencesExtension() : this(new HashSet<TypeInfo> {Support<string>.Metadata}) {}

		/// <summary>
		/// Creates a new instance with the specified blacklist and empty whitelist.
		/// </summary>
		/// <param name="blacklist">The list of prohibited types.</param>
		public DefaultReferencesExtension(ICollection<TypeInfo> blacklist) : this(blacklist, new HashSet<TypeInfo>()) {}

		/// <summary>
		/// Creates a new instance with provided types.
		/// </summary>
		/// <param name="blacklist">The list of prohibited types.</param>
		/// <param name="whitelist">The list of allowed types.</param>
		public DefaultReferencesExtension(ICollection<TypeInfo> blacklist, ICollection<TypeInfo> whitelist)
		{
			Blacklist = blacklist;
			Whitelist = whitelist;
		}

		/// <summary>
		/// The current list of prohibited types.
		/// </summary>
		public ICollection<TypeInfo> Blacklist { get; }

		/// <summary>
		/// The current list of allowed types.
		/// </summary>
		public ICollection<TypeInfo> Whitelist { get; }

		/// <inheritdoc />
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