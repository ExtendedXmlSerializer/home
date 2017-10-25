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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	using System.Reflection;

	public static class Extensions
	{
		public static XElement Member(this XElement @this, string name)
			=> @this.Element(XName.Get(name, @this.Name.NamespaceName));

		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(
			this IMemberConfiguration<T, TMember> @this, Func<TMember, bool> when)
		{
			@this.Root.With<MemberFormatExtension>().Specifications[((ISource<MemberInfo>)@this).Get()] =
				new AttributeSpecification(new DelegatedSpecification<TMember>(when).Adapt());
			return @this.Attribute();
		}

		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<MemberFormatExtension>().Registered.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Content<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<MemberFormatExtension>().Registered.Remove(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
															   Action<System.Xml.XmlWriter, T> serializer,
															   Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
															   IExtendedXmlCustomSerializer<T> serializer)
		{
			@this.Root.With<CustomXmlExtension>().Assign(@this.Get(), new Adapter<T>(serializer));
			return @this;
		}

		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
														   ICommand<XElement> migration)
			=> @this.AddMigration(migration.Execute);

		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
														   Action<XElement> migration)
			=> @this.AddMigration(migration.Yield());

		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
														   IEnumerable<Action<XElement>> migrations)
		{
			@this.Root.With<MigrationsExtension>().Add(@this.Get(), migrations.Fixed());
			return @this;
		}

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this)
			=> @this.Extend(AutoMemberFormatExtension.Default);

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this, int maxTextLength)
			=> @this.Extend(new AutoMemberFormatExtension(maxTextLength));

		public static IConfigurationContainer EnableClassicMode(this IConfigurationContainer @this)
			=> @this.Emit(EmitBehaviors.Classic).Extend(ClassicExtension.Default);

		public static IConfigurationContainer UseOptimizedNamespaces(this IConfigurationContainer @this)
			=> @this.Extend(OptimizedNamespaceExtension.Default);


		readonly static Func<Stream> New = Activators.Default.New<MemoryStream>;

		readonly static IXmlWriterFactory WriterFactory
			= new XmlWriterFactory(CloseSettings.Default.Get(Defaults.WriterSettings));

		readonly static XmlReaderSettings CloseRead = CloseSettings.Default.Get(Defaults.ReaderSettings);

		public static IExtendedXmlSerializer Create<T>(this T @this, Func<T, IConfigurationContainer> configure)
			where T : IConfigurationContainer => configure(@this).Create();


		public static string Serialize(this IExtendedXmlSerializer @this, object instance)
			=> Serialize(@this, WriterFactory, New, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, object instance)
			=> Serialize(@this, new XmlWriterFactory(CloseSettings.Default.Get(settings)), New, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, Stream stream, object instance)
			=> Serialize(@this, XmlWriterFactory.Default, stream.Self, instance);

		public static string Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, Stream stream,
									   object instance)
			=> Serialize(@this, new XmlWriterFactory(settings), stream.Self, instance);

		static string Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, Func<Stream> stream,
								object instance)
			=> new InstanceFormatter(@this, factory, stream).Get(instance);

		public static void Serialize(this IExtendedXmlSerializer @this, TextWriter writer, object instance)
			=> Serialize(@this, XmlWriterFactory.Default, writer, instance);

		public static void Serialize(this IExtendedXmlSerializer @this, XmlWriterSettings settings, TextWriter writer,
									 object instance)
			=> Serialize(@this, new XmlWriterFactory(settings), writer, instance);

		static void Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, TextWriter writer, object instance)
			=> @this.Serialize(factory.Get(writer), instance);

		public static XmlParserContext Context(this XmlNameTable @this)
			=> XmlParserContexts.Default.Get(@this ?? new NameTable());


		public static T Deserialize<T>(this IExtendedXmlSerializer @this, string data)
			=> Deserialize<T>(@this, CloseRead, data);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, string data)
			=> Deserialize<T>(@this, settings, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, Stream stream)
			=> Deserialize<T>(@this, Defaults.ReaderSettings, stream);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, Stream stream)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings, settings.NameTable.Context()), stream);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, Stream stream)
			=> @this.Deserialize(factory.Get(stream)).AsValid<T>();

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, TextReader reader)
			=> Deserialize<T>(@this, Defaults.ReaderSettings, reader);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, TextReader reader)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings, settings.NameTable.Context()), reader);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, TextReader reader)
			=> @this.Deserialize(factory.Get(reader)).AsValid<T>();

		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this, params Type[] types)
			=> EnableImplicitTyping(@this, new HashSet<Type>(types));

		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this, ICollection<Type> types)
			=> @this.Extend(new ImplicitTypingExtension(types));

		sealed class CloseSettings : IAlteration<XmlWriterSettings>, IAlteration<XmlReaderSettings>
		{
			public static CloseSettings Default { get; } = new CloseSettings();
			CloseSettings() {}

			public XmlWriterSettings Get(XmlWriterSettings parameter)
			{
				var result = parameter.Clone();
				result.CloseOutput = true;
				return result;
			}

			public XmlReaderSettings Get(XmlReaderSettings parameter)
			{
				var result = parameter.Clone();
				result.CloseInput = true;
				return result;
			}
		}
	}
}