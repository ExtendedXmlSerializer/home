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
using ExtendedXmlSerialization.ContentModel;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Properties;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;
using XmlReader = ExtendedXmlSerialization.ContentModel.Xml.XmlReader;

namespace ExtendedXmlSerialization.ExtensionModel
{
	sealed class MigrationsExtension : TypedTable<IEnumerable<Action<XElement>>>, ISerializerExtension
	{
		public MigrationsExtension() : this(new Dictionary<TypeInfo, IEnumerable<Action<XElement>>>()) {}

		public MigrationsExtension(IDictionary<TypeInfo, IEnumerable<Action<XElement>>> store) : base(store) {}

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IContents>(Register);

		IContents Register(IServiceProvider services, IContents contents)
			=>
				new Contents(services.Get<IGenericTypes>(), services.Get<ITypes>(), services.Get<ITypeProperty>(),
				             services.Get<IItemTypeProperty>(), services.Get<IArgumentsProperty>(), this, contents);

		void ICommand<IServices>.Execute(IServices parameter) {}

		public void Add(TypeInfo key, params Action<XElement>[] items)
		{
			var current = Get(key)?.ToArray() ?? Enumerable.Empty<Action<XElement>>();
			Assign(key, current.Appending(items).Fixed());
		}

		sealed class Contents : IContents
		{
			readonly IGenericTypes _genericTypes;
			readonly ITypes _types;
			readonly ITypeProperty _type;
			readonly IItemTypeProperty _item;
			readonly IArgumentsProperty _arguments;
			readonly ITypedTable<IEnumerable<Action<XElement>>> _migrations;
			readonly IContents _contents;

			public Contents(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
			                IArgumentsProperty arguments, ITypedTable<IEnumerable<Action<XElement>>> migrations,
			                IContents contents)
			{
				_genericTypes = genericTypes;
				_types = types;
				_type = type;
				_item = item;
				_arguments = arguments;
				_migrations = migrations;
				_contents = contents;
			}

			public ISerializer Get(TypeInfo parameter)
			{
				var migrations = _migrations.Get(parameter);
				var content = _contents.Get(parameter);
				var result = migrations != null
					? new Serializer(
						new Migrator(_genericTypes, _types, _type, _item, _arguments, migrations.ToImmutableArray()),
						content)
					: content;
				return result;
			}

			interface IMigrator : IAlteration<IXmlReader>, IWriter {}

			class Migrator : IMigrator
			{
				readonly static MigrationVersionProperty Property = MigrationVersionProperty.Default;

				readonly IGenericTypes _genericTypes;
				readonly ITypes _types;
				readonly ITypeProperty _type;
				readonly IItemTypeProperty _item;
				readonly IArgumentsProperty _arguments;
				readonly ImmutableArray<Action<XElement>> _migrations;
				readonly IMigrationVersionProperty _property;

				public Migrator(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
				                IArgumentsProperty arguments, ImmutableArray<Action<XElement>> migrations)
					: this(genericTypes, types, type, item, arguments, Property, migrations) {}

				public Migrator(IGenericTypes genericTypes, ITypes types, ITypeProperty type, IItemTypeProperty item,
				                IArgumentsProperty arguments, IMigrationVersionProperty property,
				                ImmutableArray<Action<XElement>> migrations)
				{
					_genericTypes = genericTypes;
					_types = types;
					_type = type;
					_item = item;
					_arguments = arguments;
					_migrations = migrations;
					_property = property;
				}

				public IXmlReader Get(IXmlReader parameter)
				{
					var fullName = parameter.Classification.FullName;
					var version = parameter.Contains(_property) ? _property.Get(parameter) : 0;

					var length = _migrations.Length;

					if (version > length)
					{
						throw new XmlException($"Unknown varsion number {version} for type {parameter.Classification}.");
					}
					if (_migrations == null)
					{
						throw new XmlException($"Migrations for type {fullName} is null.");
					}

					var element = XElement.Load(parameter.Get());
					for (var i = version; i < length; i++)
					{
						var index = (int) i;
						var migration = _migrations.ElementAtOrDefault(index);
						if (migration == null)
							throw new XmlException(
								$"Migrations for type {fullName} contains invalid migration at index {i}.");
						_migrations[index].Invoke(element);
					}
					var result = new XmlReader(_genericTypes, _types, _type, _item, _arguments, element.CreateReader());
					return result;
				}

				uint Version => (uint) _migrations.Length;
				public void Write(IXmlWriter writer, object instance) => _property.Write(writer, Version);
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

				public object Get(IXmlReader parameter)
				{
					var reader = _migrator.Get(parameter);
					var result = _serializer.Get(reader);
					return result;
				}

				public void Write(IXmlWriter writer, object instance)
				{
					_migrator.Write(writer, instance);
					_serializer.Write(writer, instance);
				}
			}
		}
	}
}