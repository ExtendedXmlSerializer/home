using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

// ReSharper disable TooManyDependencies

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class MigrationsExtension : TypedTable<IEnumerable<Action<XElement>>>, ISerializerExtension
	{
		public MigrationsExtension() : this(new Dictionary<TypeInfo, IEnumerable<Action<XElement>>>()) {}

		public MigrationsExtension(IDictionary<TypeInfo, IEnumerable<Action<XElement>>> store) : base(store) {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IContents>(Register);

		IContents Register(IServiceProvider services, IContents contents)
			=> new Contents(services.Get<IFormatReaders>(), services.Get<IClassification>(), this, contents);

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Contents : IContents
		{
			readonly IFormatReaders                             _factory;
			readonly IClassification                            _classification;
			readonly ITypedTable<IEnumerable<Action<XElement>>> _migrations;
			readonly IContents                                  _contents;

			// ReSharper disable once TooManyDependencies
			public Contents(IFormatReaders factory, IClassification classification,
			                ITypedTable<IEnumerable<Action<XElement>>> migrations, IContents contents)
			{
				_factory        = factory;
				_classification = classification;
				_migrations     = migrations;
				_contents       = contents;
			}

			public ContentModel.ISerializer Get(TypeInfo parameter)
			{
				var migrations = _migrations.Get(parameter);
				var content    = _contents.Get(parameter);
				var result = migrations != null
					             ? new
						             Serializer(new Migrator(_factory, parameter, _classification, migrations.ToImmutableArray()),
						                        content)
					             : content;
				return result;
			}

			interface IMigrator : IAlteration<IFormatReader>, IWriter {}

			sealed class Migrator : IMigrator
			{
				readonly static MigrationVersionIdentity Identity = MigrationVersionIdentity.Default;

				readonly IFormatReaders                   _factory;
				readonly TypeInfo                         _type;
				readonly IClassification                  _classification;
				readonly ImmutableArray<Action<XElement>> _migrations;
				readonly uint                             _version;
				readonly IProperty<uint>                  _property;

				public Migrator(IFormatReaders factory, TypeInfo type, IClassification classification,
				                ImmutableArray<Action<XElement>> migrations)
					: this(factory, type, classification, Identity, migrations, (uint)migrations.Length) {}

				public Migrator(IFormatReaders factory, TypeInfo type, IClassification classification,
				                IProperty<uint> property, ImmutableArray<Action<XElement>> migrations, uint version)
				{
					_factory        = factory;
					_type           = type;
					_classification = classification;
					_migrations     = migrations;
					_version        = version;
					_property       = property;
				}

				public IFormatReader Get(IFormatReader parameter)
				{
					var typeInfo = _classification.Get(parameter) ?? _type;
					var fullName = typeInfo.FullName;
					var version  = parameter.IsSatisfiedBy(_property) ? _property.Get(parameter) : 0;

					if (version > _version)
					{
						throw new XmlException($"Unknown varsion number {version} for type {typeInfo}.");
					}

					parameter.Set();

					var reader  = parameter.Get().AsValid<System.Xml.XmlReader>();
					var element = XElement.Load(reader.ReadSubtree());
					for (var i = version; i < _version; i++)
					{
						var index     = (int)i;
						var migration = _migrations.ElementAtOrDefault(index);
						if (migration == null)
							throw new XmlException(
							                       $"Migrations for type {fullName} contains invalid migration at index {i}.");
						_migrations[index]
							.Invoke(element);
					}

					var xmlReader = element.CreateReader();
					XmlParserContexts.Default.Assign(xmlReader.NameTable,
					                                 XmlParserContexts.Default.Get(reader.NameTable));
					var result    = _factory.Get(xmlReader);
					AssociatedReaders.Default.Assign(result, parameter);
					return result;
				}

				public void Write(IFormatWriter writer, object instance) => _property.Write(writer, _version);
			}

			sealed class Serializer : ContentModel.ISerializer
			{
				readonly IMigrator                _migrator;
				readonly ContentModel.ISerializer _serializer;

				public Serializer(IMigrator migrator, ContentModel.ISerializer serializer)
				{
					_migrator   = migrator;
					_serializer = serializer;
				}

				public object Get(IFormatReader parameter)
				{
					var reader = _migrator.Get(parameter);
					var result = _serializer.Get(reader);
					return result;
				}

				public void Write(IFormatWriter writer, object instance)
				{
					_migrator.Write(writer, instance);
					_serializer.Write(writer, instance);
				}
			}
		}
	}
}