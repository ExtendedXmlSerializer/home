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
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerialization.Configuration
{
	class ExtendedXmlTypeConfiguration<T> : IExtendedXmlTypeConfiguration<T>, IExtendedXmlTypeConfiguration
	{
		/// <summary>
		/// Gets the dictionary of migartions
		/// </summary>
		readonly Dictionary<int, Action<XElement>> _migrations = new Dictionary<int, Action<XElement>>();

		Func<XElement, T> _deserialize;
		Action<XmlWriter, T> _serializer;

		public IExtendedXmlPropertyConfiguration GetPropertyConfiguration(string name)
		{
			return _cache.ContainsKey(name) ? _cache[name] : null;
		}

		/// <summary>
		/// Gets the version of object
		/// </summary>
		public int Version { get; private set; }

	    public string Name { get; private set; }

	    public void Map(Type targetType, XElement currentNode)
		{
			var currentNodeVer = 0;
			var ver = currentNode.Attribute("ver");
			if (ver != null)
			{
				currentNodeVer = int.Parse(ver.Value);
			}
			if (currentNodeVer > Version)
			{
				throw new XmlException($"Unknown varsion number {currentNodeVer} for type {targetType.FullName}.");
			}
			if (_migrations == null)
				throw new XmlException($"Dictionary of migrations for type {targetType.FullName} is null.");

			for (var i = currentNodeVer; i < Version; i++)
			{
				var versionNum = i;
				if (!_migrations.ContainsKey(i))
					throw new XmlException(
						$"Dictionary of migrations for type {targetType.FullName} does not contain {versionNum} migration.");
				if (_migrations[i] == null)
					throw new XmlException(
						$"Dictionary of migrations for type {targetType.FullName} contains invalid element in position {versionNum}.");
				_migrations[i](currentNode);
			}
		}

		public object ReadObject(XElement element)
		{
			return _deserialize(element);
		}

		public void WriteObject(XmlWriter writer, object obj)
		{
			_serializer(writer, (T) obj);
		}


		public bool IsCustomSerializer { get; set; }
		public bool IsObjectReference { get; set; }
		public Func<object, string> GetObjectId { get; set; }

		readonly Dictionary<string, IExtendedXmlPropertyConfiguration> _cache =
			new Dictionary<string, IExtendedXmlPropertyConfiguration>();

		public IExtendedXmlPropertyConfiguration<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property)
		{
			var propertyConfig = new ExtendedXmlPropertyConfiguration<T, TProperty>
			                     {
				                     TypeConfiguration = this,
				                     PropertyExpression = property
			                     };
			//TODO maybe something smarter.
			var path = property.Body.ToString();
			var binding = path.Substring(path.IndexOf(".", StringComparison.OrdinalIgnoreCase) + 1);

			_cache.Add(binding, propertyConfig);
			return propertyConfig;
		}

		public IExtendedXmlTypeConfiguration<T> CustomSerializer(Action<XmlWriter, T> serializer,
		                                                         Func<XElement, T> deserialize)
		{
			IsCustomSerializer = true;
			_serializer = serializer;
			_deserialize = deserialize;
			return this;
		}

		public IExtendedXmlTypeConfiguration<T> CustomSerializer(IExtendedXmlCustomSerializer<T> serializer)
		{
			IsCustomSerializer = true;
			_serializer = serializer.Serializer;
			_deserialize = serializer.Deserialize;
			return this;
		}

		public IExtendedXmlTypeConfiguration<T> AddMigration(Action<XElement> migration)
		{
			_migrations.Add(Version++, migration);
			return this;
		}

		public IExtendedXmlTypeConfiguration<T> AddMigration(IExtendedXmlTypeMigrator migrator)
		{
			foreach (var allMigration in migrator.GetAllMigrations())
			{
				_migrations.Add(Version++, allMigration);
			}
			return this;
		}

	    public IExtendedXmlTypeConfiguration<T> EnableReferences()
	    {
	        IsObjectReference = true;
	        return this;
	    }

	    IExtendedXmlTypeConfiguration<T> IExtendedXmlTypeConfiguration<T>.Name(string name)
        {
            Name = name;
            return this;
        }
    }
}