using ExtendedXmlSerializer.ExtensionModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.Configuration
{
	/// <summary>
	/// Root-level component that is used to create serializers.  The configuration container contains all the applied
	/// configurations which are then applied when the <see cref="ContextBase.Create"/> is called, creating the fully
	/// configured serializer. This is considered the entry component for ExtendedXmlSerializer and is used extensively for
	/// configuration and resulting creation.
	/// </summary>
	public class ConfigurationContainer : ContextBase, IConfigurationContainer
	{
		readonly IRootContext _context;

		/// <summary>
		/// Creates a instance, using <see cref="DefaultExtensions.Default"/> as the set of extensions to use.
		/// </summary>
		public ConfigurationContainer() : this(DefaultExtensions.Default.ToArray()) {}

		/// <summary>
		/// Creates a new instance with the provided set of extensions.
		/// </summary>
		/// <param name="extensions">The initial set of extensions to populate the container.</param>
		public ConfigurationContainer(params ISerializerExtension[] extensions)
			: this(new ExtensionCollection(extensions)) {}

		/// <inheritdoc />
		public ConfigurationContainer(IExtensionCollection extensions) : this(new RootContext(extensions)) {}

		/// <inheritdoc />
		public ConfigurationContainer(ITypeConfigurationContext parent) : base(parent) => _context = parent.Root;

		/// <inheritdoc />
		public ConfigurationContainer(IRootContext context) : base(context) => _context = context;

		/// <inheritdoc />
		public IConfigurationContainer Extend(ISerializerExtension extension)
		{
			var existing = _context.SingleOrDefault(extension.GetType().IsInstanceOfType);
			if (existing != null)
			{
				_context.Remove(existing);
			}

			return _context.Apply(extension).Return(this);
		}

		/// <inheritdoc />
		public IEnumerator<ITypeConfiguration> GetEnumerator() => _context.Types.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}