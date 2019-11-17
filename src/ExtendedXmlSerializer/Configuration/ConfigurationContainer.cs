using ExtendedXmlSerializer.ExtensionModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExtendedXmlSerializer.Configuration
{
	public class ConfigurationContainer : ContextBase, IConfigurationContainer
	{
		readonly IRootContext _context;

		public ConfigurationContainer() : this(DefaultExtensions.Default.ToArray()) {}

		public ConfigurationContainer(params ISerializerExtension[] extensions)
			: this(new ExtensionCollection(extensions)) {}

		public ConfigurationContainer(IExtensionCollection extensions) : this(new RootContext(extensions)) {}

		public ConfigurationContainer(ITypeConfigurationContext parent) : base(parent) => _context = parent.Root;

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