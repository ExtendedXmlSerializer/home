using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public static class ExtensionMethods
	{
		public static IConfigurationElement DiscoverDeclaredContentSerializers<T>(this IConfigurationElement @this)
			=> @this.DiscoverDeclaredContentSerializers(new PublicTypesInSameNamespace<T>());

		public static IConfigurationElement DiscoverDeclaredContentSerializers(
			this IConfigurationElement @this, IEnumerable<TypeInfo> candidates)
			=> @this.DiscoverDeclaredContentSerializers(new Metadata(candidates));

		public static IConfigurationElement DiscoverDeclaredContentSerializers(
			this IConfigurationElement @this, IEnumerable<MemberInfo> candidates)
			=> @this.Extend<DeclaredMetadataContentExtension>()
			        .Executed(candidates)
			        .Return(@this);

		public static IConfigurationElement EnableThreadProtection(this IConfigurationElement @this)
			=> @this.Extended<ThreadProtectionExtension>();

		public static T EnableRootInstances<T>(this T @this) where T : class, IConfigurationElement
			=> @this.Extend<RootInstanceExtension>().Return(@this);

		public static MemberInfo Member(this IMemberConfiguration @this) => @this.Get().Get();

		public static TypeInfo Type(this ITypeConfiguration @this) => @this.Get().Get().To<TypeInfo>();

		public static IType<T> Register<T, TSerializer>(this IConfigurationElement @this)
			where TSerializer : ISerializer<T> => @this.Type<T>()
			                                           .Register(typeof(TSerializer));

		public static IType<T> Register<T>(this IType<T> @this, ContentModel.ISerializer<T> serializer)
			=> Register(@this, serializer.Adapt());

		public static IType<T> Register<T>(this IType<T> @this, ISerializer serializer)
			=> @this.Register(new ContentSerializerAdapter<T>(serializer.Adapt<T>()));

		public static IType<T> Register<T>(this IType<T> @this, IContentSerializer<T> serializer)
			=> RegisteredContentSerializers<T>.Default.Assign(@this, serializer).Return(@this);

		public static IType<T> Register<T>(this IType<T> @this, IService<IContentSerializer<T>> service)
			=> RegisteredContentSerializers<T>.Default.Assign(@this, service).Return(@this);

		public static IType<T> Register<T, TSerializer>(this IType<T> @this, A<TSerializer> _)
			where TSerializer : class, IContentSerializer<T>
			=> @this.Register(A<ActivatedContentSerializer<T, TSerializer>>.Default.Get());

		public static IType<T> Register<T>(this IType<T> @this, Type serializerType)
			=> RegisteredContentSerializers<T>.Default.Assign(@this, serializerType).Return(@this);

		public static IType<T> Unregister<T>(this IType<T> @this)
			=> RegisteredContentSerializers<T>.Default.Remove(@this.Get()).Return(@this);

		public static MemberConfiguration<T, TMember> Register<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                   Type serializerType)
			=> RegisteredContentSerializers<TMember>.Default.Assign(@this, serializerType).Return(@this);

		public static MemberConfiguration<T, TMember> Register<T, TMember, TSerializer>(
			this MemberConfiguration<T, TMember> @this, A<TSerializer> _)
			where TSerializer : class, IContentSerializer<TMember>
			=> RegisteredContentSerializers<TMember>.Default
			                                        .Assign(@this,
			                                                A<ActivatedContentSerializer<TMember, TSerializer>>.Default)
			                                        .Return(@this);

		public static MemberConfiguration<T, TMember> Register<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                   IContentSerializer<TMember> serializer)
			=> RegisteredContentSerializers<TMember>.Default.Assign(@this, serializer).Return(@this);

		public static MemberConfiguration<T, TMember> Unregister<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> RegisteredContentSerializers<TMember>.Default.Remove(@this.Get()).Return(@this);
	}
}