using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Content.Registration;
using ExtendedXmlSerializer.ExtensionModel.Services;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using ExtendedXmlSerializer.ReflectionModel;
using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;

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
			=> @this.Register(serializer.Adapt());

		public static IType<T> Register<T>(this IType<T> @this, ISerializer serializer)
			=> @this.Register(new ContentSerializerAdapter<T>(serializer.Adapt<T>()));

		public static IType<T> Register<T>(this IType<T> @this, IContentSerializer<T> serializer)
			=> @this.Set(RegisteredSerializersProperty<T>.Default, serializer).Return(@this);

		/*public static IType<T> Register<T>(this IMetadataConfiguration @this, IContentSerializer<T> serializer) where T : class, IMetadataConfiguration
			=> @this.Set(RegisteredSerializersProperty<T>.Default, serializer).Return(@this);*/

		public static IMetadataConfiguration Register<T>(this IMetadataConfiguration @this, IService<IContentSerializer<T>> service)
			=> @this.Set(RegisteredSerializersProperty<T>.Default, service);

		public static IType<T> Register<T>(this IType<T> @this, IService<IContentSerializer<T>> service)
			=> @this.Set(RegisteredSerializersProperty<T>.Default, service).Return(@this);

		public static IType<T> Register<T, TSerializer>(this IType<T> @this, A<TSerializer> _)
			where TSerializer : class, IContentSerializer<T>
			=> @this.Register(A<ActivatedContentSerializer<T, TSerializer>>.Default.Get());

		public static IType<T> Register<T>(this IType<T> @this, Type serializerType)
			=> @this.Set(RegisteredSerializersProperty<T>.Default, serializerType).Return(@this);

		public static IType<T> Unregister<T>(this IType<T> @this)
			=> @this.Entry(RegisteredSerializersProperty<T>.Default).Remove.Executed().Return(@this);

		public static MemberConfiguration<T, TMember> Register<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                   Type serializerType)
			=> @this.Set(RegisteredSerializersProperty<TMember>.Default, serializerType).Return(@this);

		public static MemberConfiguration<T, TMember> Register<T, TMember, TSerializer>(
			this MemberConfiguration<T, TMember> @this, A<TSerializer> _)
			where TSerializer : class, IContentSerializer<TMember>
			=> @this.Set(RegisteredSerializersProperty<TMember>.Default,
			             A<ActivatedContentSerializer<TMember, TSerializer>>.Default)
			        .Return(@this);

		public static MemberConfiguration<T, TMember> Register<T, TMember>(this MemberConfiguration<T, TMember> @this,
		                                                                   IContentSerializer<TMember> serializer)
			=> @this.Set(RegisteredSerializersProperty<TMember>.Default, serializer).Return(@this);

		public static MemberConfiguration<T, TMember> Unregister<T, TMember>(this MemberConfiguration<T, TMember> @this)
			=> @this.Entry(RegisteredSerializersProperty<TMember>.Default).Remove.Executed().Return(@this);
	}
}