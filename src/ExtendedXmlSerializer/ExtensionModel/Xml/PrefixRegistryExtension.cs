using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	/// <summary>
	/// An extension used to customize prefix registration.
	/// </summary>
	/// <seealso href="https://github.com/ExtendedXmlSerializer/home/issues/199"/>
	public sealed class PrefixRegistryExtension : ISerializerExtension
	{
		readonly IDictionary<Type, string> _registry;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="registry">The registry store.</param>
		public PrefixRegistryExtension(IDictionary<Type, string> registry) => _registry = registry;

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IFormatWriters, FormatWriters>()
			            .RegisterInstance<IPrefixRegistry>(new PrefixRegistry(_registry));

		void ICommand<IServices>.Execute(IServices parameter) {}

		interface IPrefixRegistry : IParameterizedSource<Type, string> {}

		sealed class PrefixRegistry : TableSource<Type, string>, IPrefixRegistry
		{
			public PrefixRegistry(IDictionary<Type, string> store) : base(store) {}
		}

		sealed class FormatWriters : IFormatWriters
		{
			readonly IPrefixRegistry                      _registry;
			readonly ITypes                               _types;
			readonly IFormatWriters _writers;

			public FormatWriters(IPrefixRegistry registry, ITypes types, IFormatWriters writers)
			{
				_registry = registry;
				_types    = types;
				_writers  = writers;
			}

			public IFormatWriter Get(System.Xml.XmlWriter parameter)
				=> new Writer(_registry, _types, _writers.Get(parameter), parameter);

			sealed class Writer : IFormatWriter
			{
				readonly IPrefixRegistry      _registry;
				readonly ITypes               _types;
				readonly IFormatWriter        _writer;
				readonly System.Xml.XmlWriter _native;

				// ReSharper disable once TooManyDependencies
				public Writer(IPrefixRegistry registry, ITypes types, IFormatWriter writer, System.Xml.XmlWriter native)
				{
					_registry = registry;
					_types    = types;
					_writer   = writer;
					_native   = native;
				}

				public object Get() => _writer.Get();

				public IIdentity Get(string name, string identifier) => _writer.Get(name, identifier);

				public void Dispose()
				{
					_writer.Dispose();
				}

				public string Get(MemberInfo parameter) => _writer.Get(parameter);

				public void Start(IIdentity identity)
				{
					if (!Write(identity))
					{
						_writer.Start(identity);
					}
				}

				bool Write(IIdentity identity)
				{
					var prefix = _registry.Get(_types.Get(identity));
					if (prefix != null)
					{
						var identifier = identity.Identifier.NullIfEmpty();
						if (identifier != null)
						{
							_native.WriteStartElement(prefix, identity.Name, identifier);
							return true;
						}
					}

					return false;
				}

				public void EndCurrent()
				{
					_writer.EndCurrent();
				}

				public void Content(IIdentity identity, string content)
				{
					_writer.Content(identity, content);
				}

				public void Content(string content)
				{
					_writer.Content(content);
				}

				public void Verbatim(string content)
				{
					_writer.Verbatim(content);
				}
			}
		}
	}
}