using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Members;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ExtensionModel.Xml.Classic;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using XmlWriter = System.Xml.XmlWriter;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this)
			=> @this.Extend(AutoMemberFormatExtension.Default);

		public static IConfigurationContainer UseAutoFormatting(this IConfigurationContainer @this, int maxTextLength)
			=> @this.Extend(new AutoMemberFormatExtension(maxTextLength));

		public static IConfigurationContainer UseOptimizedNamespaces(this IConfigurationContainer @this)
			=> @this.Extend(RootInstanceExtension.Default)
			        .Extend(OptimizedNamespaceExtension.Default);

		public static IConfigurationContainer WithValidCharacters(this IConfigurationContainer @this)
			=> @this.Type<string>().Alter(ValidContentCharacters.Default.Get);

		public static ITypeConfiguration<T> UseClassicSerialization<T>(this ITypeConfiguration<T> @this)
			where T : ISerializable
			=> @this.Register(Support<ClassicSerializationAdapter<T>>.Key);

		public static IMemberConfiguration<T, string> WithValidCharacters<T>(this IMemberConfiguration<T, string> @this)
			=> @this.Alter(ValidContentCharacters.Default.Get);

		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(
			this IMemberConfiguration<T, TMember> @this,
			Func<TMember, bool> when)
		{
			@this.Root.With<MemberFormatExtension>()
			     .Specifications[@this.GetMember()] =
				new AttributeSpecification(new DelegatedSpecification<TMember>(when).Adapt());
			return @this.Attribute();
		}

		public static IMemberConfiguration<T, TMember> Attribute<T, TMember>(
			this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<MemberFormatExtension>()
			        .Registered.Apply(@this.GetMember())
			        .Return(@this);

		public static IMemberConfiguration<T, TMember> Content<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Root.With<MemberFormatExtension>()
			        .Registered.Remove(@this.GetMember())
			        .Return(@this);

		public static IMemberConfiguration<T, string> Verbatim<T>(this IMemberConfiguration<T, string> @this)
			=> @this.Register(VerbatimContentSerializer.Default);

		#region v1

		public static ITypeConfiguration<T> CustomSerializer<T, TSerializer>(this IConfigurationContainer @this)
			where TSerializer : IExtendedXmlCustomSerializer<T>
			=> @this.CustomSerializer<T>(typeof(TSerializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this IConfigurationContainer @this, Type serializerType)
			=> @this.Type<T>()
			        .CustomSerializer(new ActivatedXmlSerializer(serializerType, Support<T>.Metadata));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        Action<XmlWriter, T> serializer,
		                                                        Func<XElement, T> deserialize)
			=> @this.CustomSerializer(new ExtendedXmlCustomSerializer<T>(deserialize, serializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer<T> serializer)
			=> @this.CustomSerializer(new Adapter<T>(serializer));

		public static ITypeConfiguration<T> CustomSerializer<T>(this ITypeConfiguration<T> @this,
		                                                        IExtendedXmlCustomSerializer serializer)
			=> @this.Root.With<CustomSerializationExtension>()
			        .XmlSerializers.Apply(@this.Get(), serializer)
			        .Return(@this);

		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
		                                                    ICommand<XElement> migration)
			=> @this.AddMigration(migration.Execute);

		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
		                                                    Action<XElement> migration)
			=> @this.AddMigration(migration.Yield());

		public static ITypeConfiguration<T> AddMigration<T>(this ITypeConfiguration<T> @this,
		                                                    IEnumerable<Action<XElement>> migrations)
			=> @this.Root.With<MigrationsExtension>()
			        .Apply(@this.Get(), migrations.Fixed())
			        .Return(@this);

		#endregion
	}
}