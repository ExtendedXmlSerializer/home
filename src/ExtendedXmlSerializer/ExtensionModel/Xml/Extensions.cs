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
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	public static class Extensions
	{
		public static XElement Member(this XElement @this, string name)
			=> @this.Element(XName.Get(name, @this.Name.NamespaceName));

		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(
			this IMemberConfiguration<T, TMember> @this, Func<TMember, bool> when)
		{
			@this.Root.With<MemberFormatExtension>()
			     .Specifications[((ISource<MemberInfo>)@this).Get()] =
				new AttributeSpecification(new DelegatedSpecification<TMember>(when).Adapt());
			return @this.Attribute();
		}

		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<MemberFormatExtension>()
			     .Registered.Add(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IMemberConfiguration<T, string> Verbatim<T>(this IMemberConfiguration<T, string> @this) =>
			@this.Register(VerbatimContentSerializer.Default);

		public static ITypeConfiguration<T> Register<T, TSerializer>(this IConfigurationContainer @this)
			where TSerializer : ISerializer<T> => @this.Type<T>()
			                                           .Register(typeof(TSerializer));

		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, Type serializerType)
			=> @this.Register(new ActivatedSerializer(serializerType, Support<T>.Key));

		public static ITypeConfiguration<T> Alter<T>(this ITypeConfiguration<T> @this, Func<T, T> write) =>
			Alter(@this, Self<T>.Default.Get, write);

		public static ITypeConfiguration<T> Alter<T>(this ITypeConfiguration<T> @this, Func<T, T> read, Func<T, T> write)
			=> @this.Alter(new DelegatedAlteration<T>(read), new DelegatedAlteration<T>(write));

		public static ITypeConfiguration<T> Alter<T>(this ITypeConfiguration<T> @this, IAlteration<T> read,
		                                             IAlteration<T> write)
		{
			@this.Root.With<AlteredContentExtension>()
			     .Types.Assign(Support<T>.Key, new ContentAlteration(read.Adapt(), write.Adapt()));
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Alter<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 Func<TMember, TMember> write)
			=> Alter(@this, Self<TMember>.Default.Get, write);

		public static IMemberConfiguration<T, TMember> Alter<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 Func<TMember, TMember> read,
		                                                                 Func<TMember, TMember> write)
			=> @this.Alter(new DelegatedAlteration<TMember>(read), new DelegatedAlteration<TMember>(write));

		public static IMemberConfiguration<T, TMember> Alter<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 IAlteration<TMember> read,
		                                                                 IAlteration<TMember> write)
		{
			@this.Root.With<AlteredContentExtension>()
			     .Members.Assign(@this.GetMember(), new ContentAlteration(read.Adapt(), write.Adapt()));
			return @this;
		}

		public static MemberInfo GetMember(this IMemberConfiguration @this) => @this.To<ISource<MemberInfo>>()
		                                                                            .Get();

		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, ISerializer<T> serializer) =>
			Register(@this, serializer.Adapt());

		public static ITypeConfiguration<T> Register<T>(this ITypeConfiguration<T> @this, ISerializer serializer)
		{
			@this.Root.With<CustomSerializationExtension>()
			     .Types.Assign(@this.Get(), serializer);
			return @this;
		}

		public static ITypeConfiguration<T> Unregister<T>(this ITypeConfiguration<T> @this)
		{
			@this.Root.With<CustomSerializationExtension>()
			     .Types.Remove(@this.Get());
			return @this;
		}

		public static ITypeConfiguration<T> CustomSerializer<T, TSerializer>(this IConfigurationContainer @this)
			where TSerializer : IExtendedXmlCustomSerializer<T>
			=> @this.CustomSerializer<T>(typeof(TSerializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this IConfigurationContainer @this, Type serializerType)
			=> @this.Type<T>()
			        .CustomSerializer(new ActivatedXmlSerializer(serializerType, Support<T>.Key));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        Action<System.Xml.XmlWriter, T> serializer,
		                                                        Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer<T> serializer)
			=> @this.CustomSerializer(new Adapter<T>(serializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer serializer)
		{
			@this.Root.With<CustomSerializationExtension>()
			     .XmlSerializers.Assign(@this.Get(), serializer);
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    Type serializerType)
			=> @this.Register(new ActivatedSerializer(serializerType, Support<TMember>.Key));

		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    ISerializer<TMember> serializer) =>
			Register(@this, serializer.Adapt());

		public static IMemberConfiguration<T, TMember> Register<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                    ISerializer serializer)
		{
			@this.Root.With<CustomSerializationExtension>()
			     .Members.Assign(((ISource<MemberInfo>)@this).Get(), serializer);
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Unregister<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<CustomSerializationExtension>()
			     .Members.Remove(((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Content<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Root.With<MemberFormatExtension>()
			     .Registered.Remove(((ISource<MemberInfo>)@this).Get());
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
			@this.Root.With<MigrationsExtension>()
			     .Add(@this.Get(), migrations.Fixed());
			return @this;
		}

		public static IConfigurationContainer WithValidCharacters(this IConfigurationContainer @this)
			=> @this.Type<string>()
			        .Alter(ValidContentCharacters.Default.Get);

		public static IMemberConfiguration<T, string> WithValidCharacters<T>(this IMemberConfiguration<T, string> @this)
			=> @this.Alter(ValidContentCharacters.Default.Get);

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this)
			=> @this.Extend(AutoMemberFormatExtension.Default);

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this, int maxTextLength)
			=> @this.Extend(new AutoMemberFormatExtension(maxTextLength));

		public static IConfigurationContainer EnableClassicMode(this IConfigurationContainer @this)
			=> @this.Emit(EmitBehaviors.Classic)
			        .Extend(ClassicExtension.Default);

		public static IConfigurationContainer InspectingType<T>(this IConfigurationContainer @this)
			=> @this.InspectingTypes(typeof(T).Yield());

		public static IConfigurationContainer InspectingTypes(this IConfigurationContainer @this, IEnumerable<Type> types)
			=> @this.Extend(new ClassicIdentificationExtension(types.YieldMetadata()
			                                                        .ToList()));

		public static IConfigurationContainer UseOptimizedNamespaces(this IConfigurationContainer @this)
			=> @this.Extend(RootInstanceExtension.Default)
			        .Extend(OptimizedNamespaceExtension.Default);

		readonly static Func<Stream> New = DefaultActivators.Default.New<MemoryStream>;

		readonly static IXmlWriterFactory WriterFactory
			= new XmlWriterFactory(CloseSettings.Default.Get(Defaults.WriterSettings));

		readonly static XmlReaderSettings CloseRead = CloseSettings.Default.Get(Defaults.ReaderSettings);

		public static IExtendedXmlSerializer Create<T>(this T @this, Func<T, IConfigurationContainer> configure)
			where T : IConfigurationContainer => configure(@this)
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
			=> @this.Deserialize(factory.Get(stream))
			        .AsValid<T>();

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, TextReader reader)
			=> Deserialize<T>(@this, Defaults.ReaderSettings, reader);

		public static T Deserialize<T>(this IExtendedXmlSerializer @this, XmlReaderSettings settings, TextReader reader)
			=> Deserialize<T>(@this, new XmlReaderFactory(settings, settings.NameTable.Context()), reader);

		static T Deserialize<T>(this IExtendedXmlSerializer @this, IXmlReaderFactory factory, TextReader reader)
			=> @this.Deserialize(factory.Get(reader))
			        .AsValid<T>();

		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this, params Type[] types)
			=> EnableImplicitTyping(@this, types.AsEnumerable());

		public static IConfigurationContainer EnableImplicitTypingFromPublicNested<T>(this IConfigurationContainer @this) =>
			@this.EnableImplicitTyping(new PublicNestedTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromNested<T>(this IConfigurationContainer @this) =>
			@this.EnableImplicitTyping(new NestedTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromAll<T>(this IConfigurationContainer @this) =>
			@this.EnableImplicitTyping(new AllAssemblyTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromPublic<T>(this IConfigurationContainer @this) =>
			@this.EnableImplicitTyping(new PublicAssemblyTypes<T>());

		public static IConfigurationContainer EnableImplicitTypingFromNamespace<T>(this IConfigurationContainer @this) =>
			@this.EnableImplicitTyping(new AllTypesInSameNamespace<T>());

		public static IConfigurationContainer
			EnableImplicitTypingFromNamespacePublic<T>(this IConfigurationContainer @this) =>
			@this.EnableImplicitTyping(new PublicTypesInSameNamespace<T>());

		public static IConfigurationContainer EnableImplicitTyping(this IConfigurationContainer @this,
		                                                           IEnumerable<Type> types)
			=> @this.Extend(new ImplicitTypingExtension(types.ToMetadata()));

		public static IConfigurationContainer EnableXmlText(this IConfigurationContainer @this)
			=> @this.Extend(XmlTextExtension.Default);

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