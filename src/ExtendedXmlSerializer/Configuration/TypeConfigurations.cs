using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ReflectionModel;

namespace ExtendedXmlSerializer.Configuration
{
	sealed class TypeConfigurations : CacheBase<TypeInfo, ITypeConfiguration>, ITypeConfigurations
	{
		readonly IExtensionCollection                               _extensions;
		readonly IDictionary<TypeInfo, string>                      _names;
		readonly ConcurrentDictionary<TypeInfo, ITypeConfiguration> _store;

		public TypeConfigurations(IExtensionCollection extensions)
			: this(extensions, extensions.Find<TypeNamesExtension>()
			                             .Names, new ConcurrentDictionary<TypeInfo, ITypeConfiguration>()) {}

		public TypeConfigurations(IExtensionCollection extensions, IDictionary<TypeInfo, string> names,
		                          ConcurrentDictionary<TypeInfo, ITypeConfiguration> store) : base(store)
		{
			_extensions = extensions;
			_names      = names;
			_store      = store;
		}

		protected override ITypeConfiguration Create(TypeInfo parameter)
		{
			var property = new TypeProperty<string>(_names, parameter);
			var root = _extensions.Find<RootContextExtension>()
			                      .Root;
			var result = Source.Default
			                   .Get(parameter)
			                   .Invoke(root, property);
			return result;
		}

		sealed class Source : Generic<IRootContext, IProperty<string>, ITypeConfiguration>
		{
			public static Source Default { get; } = new Source();

			Source() : base(typeof(TypeConfiguration<>)) {}
		}

		public IEnumerator<ITypeConfiguration> GetEnumerator() => _store.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}