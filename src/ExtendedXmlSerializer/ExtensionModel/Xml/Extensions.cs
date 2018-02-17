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

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public static class Extensions
	{
		public static XElement Member(this XElement @this, string name)
			=> @this.Element(XName.Get(name, @this.Name.NamespaceName));

		public static MemberConfiguration<T, TMember> Attribute<T, TMember>(
			this MemberConfiguration<T, TMember> @this, Func<TMember, bool> when)
		{
			@this.Extend<MemberFormatExtension>().Specifications[@this.Member()] =
				new AttributeSpecification(new DelegatedSpecification<TMember>(when).Adapt());
			return @this.Attribute();
		}

		public static MemberConfiguration<T, TMember> Attribute<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> @this.Extend<MemberFormatExtension>()
			        .Registered.Adding(@this.Member())
			        .Return(@this);

		public static MemberConfiguration<T, string> Verbatim<T>(this MemberConfiguration<T, string> @this) =>
			@this.Register(new ContentSerializerAdapter<string>(VerbatimContentSerializer.Default));

		public static TypeConfiguration<T> Alter<T>(this TypeConfiguration<T> @this, Func<T, T> write) =>
			Alter(@this, Self<T>.Default.Get, write);

		public static TypeConfiguration<T> Alter<T>(this TypeConfiguration<T> @this, Func<T, T> read, Func<T, T> write)
			=> @this.Alter(new DelegatedAlteration<T>(read), new DelegatedAlteration<T>(write));

		public static TypeConfiguration<T> Alter<T>(this TypeConfiguration<T> @this, IAlteration<T> read,
		                                             IAlteration<T> write)
			=> @this.Extend<AlteredContentExtension>()
			        .Types.Assigned(Support<T>.Key,
			                        new ContentAlteration(read.Adapt(), write.Adapt()))
			        .Return(@this);


		public static MemberConfiguration<T, TMember> Alter<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                 Func<TMember, TMember> write)
			=> Alter(@this, Self<TMember>.Default.Get, write);

		public static MemberConfiguration<T, TMember> Alter<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                 Func<TMember, TMember> read,
		                                                                 Func<TMember, TMember> write)
			=> @this.Alter(new DelegatedAlteration<TMember>(read), new DelegatedAlteration<TMember>(write));

		public static MemberConfiguration<T, TMember> Alter<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                IAlteration<TMember> read,
		                                                                IAlteration<TMember> write)
			=> @this.Extend<AlteredContentExtension>()
			        .Members.Assigned(@this.Member(),
			                          new ContentAlteration(read.Adapt(), write.Adapt()))
			        .Return(@this);

		public static TypeConfiguration<T> CustomSerializer<T, TSerializer>(this IConfigurationElement @this)
			where TSerializer : IExtendedXmlCustomSerializer<T>
			=> @this.CustomSerializer<T>(typeof(TSerializer));

		public static TypeConfiguration<T> CustomSerializer<T>(this IConfigurationElement @this, Type serializerType)
			=> @this.Type<T>()
			        .Register(new ActivatedXmlSerializer(serializerType, Support<T>.Key));

		public static TypeConfiguration<T> CustomSerializer<T>(this TypeConfiguration<T> @this,
		                                                        Action<System.Xml.XmlWriter, T> serializer,
		                                                        Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		public static TypeConfiguration<T> CustomSerializer<T>(this TypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer<T> serializer)
			=> @this.Register(new GenericCustomXmlSerializer<T>(serializer));

		public static TypeConfiguration<T> CustomSerializer<T>(this TypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer serializer)
			=> @this.Register(new CustomXmlSerializer<T>(serializer));

		public static MemberConfiguration<T, TMember> Content<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> @this.Extend<MemberFormatExtension>()
			        .Registered.Removing(@this.Member())
			        .Return(@this);

		public static TypeConfiguration<T> AddMigration<T>(this TypeConfiguration<T> @this,
		                                                    ICommand<XElement> migration)
			=> @this.AddMigration(migration.Execute);

		public static TypeConfiguration<T> AddMigration<T>(this TypeConfiguration<T> @this,
		                                                    Action<XElement> migration)
			=> @this.AddMigration(migration.Yield());

		public static TypeConfiguration<T> AddMigration<T>(this TypeConfiguration<T> @this,
		                                                    IEnumerable<Action<XElement>> migrations)
		{
			@this.Extend<MigrationsExtension>().Add(@this.Type(), migrations.Fixed());
			return @this;
		}

		public static IConfigurationElement WithValidCharacters(this IConfigurationElement @this)
			=> @this.Type<string>()
			        .Alter(ValidContentCharacters.Default.Get);

		public static MemberConfiguration<T, string> WithValidCharacters<T>(this MemberConfiguration<T, string> @this)
			=> @this.Alter(ValidContentCharacters.Default.Get);

		public static IConfigurationElement UseAutoFormatting(this IConfigurationElement @this)
			=> @this.Extended<AutoMemberFormatExtension>();

		public static IConfigurationElement UseAutoFormatting(this IConfigurationElement @this, int maxTextLength)
			=> @this.Add.Executed(new AutoMemberFormatExtension(maxTextLength))
			        .Return(@this);

		public static IConfigurationElement EnableClassicMode(this IConfigurationElement @this)
			=> @this.Emit(EmitBehaviors.Classic)
			        .Add.Executed(ClassicExtension.Default)
			        .Return(@this);

		public static IConfigurationElement UseOptimizedNamespaces(this IConfigurationElement @this)
			=> @this.Extended<OptimizedNamespaceExtension>();


		readonly static Func<Stream> New = DefaultActivators.Default.New<MemoryStream>;

		readonly static IXmlWriterFactory WriterFactory
			= new XmlWriterFactory(CloseSettings.Default.Get(Defaults.WriterSettings));

		readonly static XmlReaderSettings CloseRead = CloseSettings.Default.Get(Defaults.ReaderSettings);

		public static IExtendedXmlSerializer Create<T>(this T @this, Func<T, IConfigurationElement> configure)
			where T : IConfigurationElement => configure(@this)
			.Create();


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

		static void Serialize(this IExtendedXmlSerializer @this, IXmlWriterFactory factory, TextWriter writer,
		                      object instance)
			=> @this.Serialize(factory.Get(writer), instance);

		public static XmlParserContext Context(this XmlNameTable @this)
			=> XmlParserContexts.Default.Get(@this);


		public static T Deserialize<T>(this IExtendedXmlSerializer @this, string data)
			=> Deserialize<T>(@this, CloseRead, data);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, string data)
			=> Deserialize<T>(@this, settings, new MemoryStream(Encoding.UTF8.GetBytes(data)));

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, Stream stream)
			=> Deserialize<T>(@this, Defaults.ReaderSettings, stream);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, Stream stream)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings, settings.NameTable.Context()), stream);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, Stream stream)
			=> @this.Deserialize(factory.Get(stream))
			        .To<T>();

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, TextReader reader)
			=> Deserialize<T>(@this, Defaults.ReaderSettings, reader);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, TextReader reader)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings, settings.NameTable.Context()), reader);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, TextReader reader)
			=> @this.Deserialize(factory.Get(reader))
			        .To<T>();

		public static IConfigurationElement EnableImplicitTyping(this IConfigurationElement @this, params Type[] types)
			=> EnableImplicitTyping(@this, types.AsEnumerable());

		public static IConfigurationElement EnableImplicitTypingFromPublicNested<T>(this IConfigurationElement @this) =>
			@this.EnableImplicitTyping(new PublicNestedTypes<T>());

		public static IConfigurationElement EnableImplicitTypingFromNested<T>(this IConfigurationElement @this) =>
			@this.EnableImplicitTyping(new NestedTypes<T>());

		public static IConfigurationElement EnableImplicitTypingFromAll<T>(this IConfigurationElement @this) =>
			@this.EnableImplicitTyping(new AllAssemblyTypes<T>());

		public static IConfigurationElement EnableImplicitTypingFromPublic<T>(this IConfigurationElement @this) =>
			@this.EnableImplicitTyping(new PublicAssemblyTypes<T>());

		public static IConfigurationElement EnableImplicitTypingFromNamespace<T>(this IConfigurationElement @this) =>
			@this.EnableImplicitTyping(new AllTypesInSameNamespace<T>());

		public static IConfigurationElement
			EnableImplicitTypingFromNamespacePublic<T>(this IConfigurationElement @this) =>
			@this.EnableImplicitTyping(new PublicTypesInSameNamespace<T>());

		public static IConfigurationElement EnableImplicitTyping(this IConfigurationElement @this,
		                                                           IEnumerable<Type> types)
			=> @this.Add.Executed(new ImplicitTypingExtension(types.ToMetadata()))
			        .Return(@this);
	}
}