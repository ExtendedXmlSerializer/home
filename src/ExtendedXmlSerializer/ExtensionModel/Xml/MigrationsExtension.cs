// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;
using IContents = ExtendedXmlSerializer.ContentModel.Content.IContents;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class MigrationsExtension : TypedTable<IEnumerable<Action<XElement>>>, ISerializerExtension
	{
		[UsedImplicitly]
		public MigrationsExtension() : this(new Dictionary<TypeInfo, IEnumerable<Action<XElement>>>()) {}

		public MigrationsExtension(IDictionary<TypeInfo, IEnumerable<Action<XElement>>> store) : base(store) {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IContents>(Register);

		IContents Register(IServiceProvider services, IContents contents)
			=> new Contents(services.Get<IFormatReaders<XmlReader>>(), services.Get<IClassification>(), this, contents);

		void ICommand<IServices>.Execute(IServices parameter) {}

		public void Add(TypeInfo key, params Action<XElement>[] items)
		{
			var current = Get(key)?.ToArray() ?? Enumerable.Empty<Action<XElement>>();
			Assign(key, current.Appending(items).Fixed());
		}

		sealed class Contents : IContents
		{
			readonly IFormatReaders<XmlReader> _factory;
			readonly IClassification _classification;
			readonly ITypedTable<IEnumerable<Action<XElement>>> _migrations;
			readonly IContents _contents;

			public Contents(IFormatReaders<XmlReader> factory, IClassification classification,
			                ITypedTable<IEnumerable<Action<XElement>>> migrations, IContents contents)
			{
				_factory = factory;
				_classification = classification;
				_migrations = migrations;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var migrations = _migrations.Get(parameter);
				var content = _contents.Get(parameter);
				var result = migrations != null
					? new Serializer(new Migrator(_factory, _classification, migrations.ToImmutableArray()), content)
					: content;
				return result;
			}

			interface IMigrator : IAlteration<IFormatReader>, IWriter {}

			sealed class Migrator : IMigrator
			{
				readonly static MigrationVersionIdentity Identity = MigrationVersionIdentity.Default;

				readonly IFormatReaders<XmlReader> _factory;
				readonly IClassification _classification;
				readonly ImmutableArray<Action<XElement>> _migrations;
				readonly IProperty<uint> _property;

				public Migrator(IFormatReaders<XmlReader> factory, IClassification classification,
				                ImmutableArray<Action<XElement>> migrations)
					: this(factory, classification, Identity, migrations) {}

				public Migrator(IFormatReaders<XmlReader> factory, IClassification classification, IProperty<uint> property,
				                ImmutableArray<Action<XElement>> migrations)
				{
					_factory = factory;
					_classification = classification;
					_migrations = migrations;
					_property = property;
				}

				public IFormatReader Get(IFormatReader parameter)
				{
					var typeInfo = _classification.Get(parameter);
					var fullName = typeInfo.FullName;
					var version = parameter.IsSatisfiedBy(_property) ? _property.Get(parameter) : 0;

					var length = _migrations.Length;

					if (version > length)
					{
						throw new XmlException($"Unknown varsion number {version} for type {typeInfo}.");
					}

					if (_migrations == null)
					{
						throw new XmlException($"Migrations for type {fullName} is null.");
					}

					var element = XElement.Load(parameter.Get().AsValid<XmlReader>());
					for (var i = version; i < length; i++)
					{
						var index = (int) i;
						var migration = _migrations.ElementAtOrDefault(index);
						if (migration == null)
							throw new XmlException(
								$"Migrations for type {fullName} contains invalid migration at index {i}.");
						_migrations[index].Invoke(element);
					}
					var result = _factory.Get(element.CreateReader());
					return result;
				}

				uint Version => (uint) _migrations.Length;
				public void Write(IFormatWriter writer, object instance) => _property.Write(writer, Version);
			}

			sealed class Serializer : ISerializer
			{
				readonly IMigrator _migrator;
				readonly ISerializer _serializer;

				public Serializer(IMigrator migrator, ISerializer serializer)
				{
					_migrator = migrator;
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