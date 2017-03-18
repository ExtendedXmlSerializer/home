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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ExtensionModel;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.Configuration
{
	public static class Extensions
	{
		public static IExtendedXmlConfiguration Apply<T>(this IExtendedXmlConfiguration @this)
			where T : class, ISerializerExtension
			=> Apply(@this, Support<T>.New);

		public static IExtendedXmlConfiguration Apply<T>(this IExtendedXmlConfiguration @this, Func<T> create)
			where T : class, ISerializerExtension
		{
			if (@this.Find<T>() == null)
			{
				@this.Add(create);
			}
			return @this;
		}

		public static T With<T>(this IExtendedXmlConfiguration @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? @this.Add<T>();

		public static T Add<T>(this IExtendedXmlConfiguration @this) where T : ISerializerExtension
			=> Add(@this, Support<T>.New);

		public static T Add<T>(this IExtendedXmlConfiguration @this, Func<T> create) where T : ISerializerExtension
		{
			var result = create();
			@this.Add(result);
			return result;
		}

		public static IExtendedXmlConfiguration EnableReferences(this IExtendedXmlConfiguration @this)
			=> @this.Apply<ReferencesExtension>();

		public static ExtendedXmlTypeConfiguration<T> ConfigureType<T>(this IExtendedXmlConfiguration @this)
			=> @this.Type<T>();

		public static ExtendedXmlTypeConfiguration<T> Type<T>(this IExtendedXmlConfiguration @this)
			=> TypeConfigurations<T>.Default.Get(@this);

		public static ExtendedXmlTypeConfiguration<T> Name<T>(this ExtendedXmlTypeConfiguration<T> @this, string name)
		{
			@this.Name.Assign(name);
			return @this;
		}

		public static IExtendedXmlTypeConfiguration GetTypeConfiguration(this IExtendedXmlConfiguration @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static IExtendedXmlTypeConfiguration GetTypeConfiguration(this IExtendedXmlConfiguration @this, TypeInfo type)
			=> TypeConfigurations.Defaults.Get(@this).Get(type);

		public static string Name<T>(this ExtendedXmlTypeConfiguration<T> @this) => @this.Name.Get();

		public static IExtendedXmlMemberConfiguration Member(this IExtendedXmlTypeConfiguration @this, string name)
		{
			var member = @this.Get().GetMember(name).SingleOrDefault();
			var result = member != null ? @this.Member(member) : null;
			return result;
		}

		public static ExtendedXmlTypeConfiguration<T> CustomSerializer<T>(
			this ExtendedXmlTypeConfiguration<T> @this,
			Action<XmlWriter, T> serializer,
			Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		public static ExtendedXmlTypeConfiguration<T> CustomSerializer<T>(this ExtendedXmlTypeConfiguration<T> @this,
		                                                                  IExtendedXmlCustomSerializer<T> serializer)
		{
			@this.Configuration.With<CustomXmlExtension>().Assign(@this.Get(), new Adapter<T>(serializer));
			return @this;
		}

		public static ExtendedXmlTypeConfiguration<T> AddMigration<T>(this ExtendedXmlTypeConfiguration<T> @this,
		                                                              Action<XElement> migration)
			=> @this.AddMigration(migration.Yield());

		public static ExtendedXmlTypeConfiguration<T> AddMigration<T>(this ExtendedXmlTypeConfiguration<T> @this,
		                                                              IEnumerable<Action<XElement>> migrations)
		{
			@this.Configuration.With<MigrationsExtension>().Add(@this.Get(), migrations.Fixed());
			return @this;
		}

		public static ExtendedXmlTypeConfiguration<T> EnableReferences<T, TMember>(this ExtendedXmlTypeConfiguration<T> @this,
		                                                                           Expression<Func<T, TMember>> member)
		{
			@this.Configuration.With<ReferencesExtension>().Assign(@this.Get(), member.GetMemberInfo());
			return @this;
		}

		public static IExtendedXmlConfiguration WithSettings(this IExtendedXmlConfiguration @this,
		                                                     XmlReaderSettings readerSettings,
		                                                     XmlWriterSettings writerSettings)
			=> @this.Extend(new XmlSerializationExtension(readerSettings, writerSettings));

		public static IExtendedXmlConfiguration WithSettings(this IExtendedXmlConfiguration @this,
		                                                     XmlReaderSettings readerSettings)
			=> @this.Extend(new XmlSerializationExtension(readerSettings));

		public static IExtendedXmlConfiguration WithSettings(this IExtendedXmlConfiguration @this,
		                                                     XmlWriterSettings writerSettings)
			=> @this.Extend(new XmlSerializationExtension(writerSettings));

		public static IExtendedXmlConfiguration Extend(this IExtendedXmlConfiguration @this,
		                                               params ISerializerExtension[] extensions)
			=> new ExtendedXmlConfiguration(@this.With(extensions));

		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions).ToArray();

		public static IExtendedXmlConfiguration UseEncryptionAlgorithm(this IExtendedXmlConfiguration @this,
		                                                               IEncryption encryption)
			=> @this.Extend(new EncryptionExtension(encryption));

		public static IExtendedXmlConfiguration UseAutoProperties(this IExtendedXmlConfiguration @this,
		                                                          int maxTextLength = 128)
			=> @this.Extend(new AutoAttributesExtension(maxTextLength));

		public static IExtendedXmlConfiguration UseOptimizedNamespaces(this IExtendedXmlConfiguration @this)
			=> @this.Extend(OptimizedNamespaceExtension.Default);
	}
}