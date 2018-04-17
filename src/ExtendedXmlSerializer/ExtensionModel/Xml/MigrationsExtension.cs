// MIT License
// 
// Copyright (c) 2016-2018 Wojciech Nagórski
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

using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Properties;
using ExtendedXmlSerializer.ContentModel.Reflection;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class MigrationsExtension : TypedTable<IEnumerable<Action<XElement>>>, ISerializerExtension
	{
		[UsedImplicitly]
		public MigrationsExtension() : this(new Dictionary<TypeInfo, IEnumerable<Action<XElement>>>()) {}

		public MigrationsExtension(IDictionary<TypeInfo, IEnumerable<Action<XElement>>> store) : base(store) {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IContents>(Register);

		IContents Register(IServiceProvider services, IContents contents)
			=>
				new Contents(services.Get<IFormatReaders<System.Xml.XmlReader>>(), services.Get<IClassification>(), this, contents);

		void ICommand<IServices>.Execute(IServices parameter) {}

		public void Add(TypeInfo key, params Action<XElement>[] items)
		{
			var current = Get(key)
				              ?.ToArray() ?? Enumerable.Empty<Action<XElement>>();
			Assign(key, current.Appending(items)
			                   .Fixed());
		}

		sealed class Contents : IContents
		{
			readonly IFormatReaders<System.Xml.XmlReader>       _factory;
			readonly IClassification                            _classification;
			readonly ITypedTable<IEnumerable<Action<XElement>>> _migrations;
			readonly IContents                                  _contents;

			public Contents(IFormatReaders<System.Xml.XmlReader> factory, IClassification classification,
			                ITypedTable<IEnumerable<Action<XElement>>> migrations, IContents contents)
			{
				_factory        = factory;
				_classification = classification;
				_migrations     = migrations;
				_contents       = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var migrations = _migrations.Get(parameter);
				var content    = _contents.Get(parameter);
				var result = migrations != null
					             ? new Serializer(new Migrator(_factory, parameter, _classification, migrations.ToImmutableArray()),
					                              content)
					             : content;
				return result;
			}

			interface IMigrator : IAlteration<IFormatReader>, IWriter {}

			sealed class Migrator : IMigrator
			{
				readonly static MigrationVersionIdentity Identity = MigrationVersionIdentity.Default;

				readonly IFormatReaders<System.Xml.XmlReader> _factory;
				readonly TypeInfo                             _type;
				readonly IClassification                      _classification;
				readonly ImmutableArray<Action<XElement>>     _migrations;
				readonly uint                                 _version;
				readonly IProperty<uint>                      _property;

				public Migrator(IFormatReaders<System.Xml.XmlReader> factory, TypeInfo type, IClassification classification,
				                ImmutableArray<Action<XElement>> migrations)
					: this(factory, type, classification, Identity, migrations, (uint)migrations.Length) {}

				public Migrator(IFormatReaders<System.Xml.XmlReader> factory, TypeInfo type, IClassification classification,
				                IProperty<uint> property,
				                ImmutableArray<Action<XElement>> migrations, uint version)
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

					var element = XElement.Load(parameter.Get()
					                                     .AsValid<System.Xml.XmlReader>()
					                                     .ReadSubtree());
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
					var result    = _factory.Get(xmlReader);
					AssociatedReaders.Default.Assign(result, parameter);
					return result;
				}

				public void Write(IFormatWriter writer, object instance) => _property.Write(writer, _version);
			}

			sealed class Serializer : ISerializer
			{
				readonly IMigrator   _migrator;
				readonly ISerializer _serializer;

				public Serializer(IMigrator migrator, ISerializer serializer)
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