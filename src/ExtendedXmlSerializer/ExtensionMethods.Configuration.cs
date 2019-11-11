using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.References;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.ReflectionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	// ReSharper disable once MismatchedFileName
	public static partial class ExtensionMethods
	{
		public static IExtendedXmlSerializer Create(this IContext @this) => @this.Root.Create();

		public static IRootContext Apply<T>(this IRootContext @this)
			where T : class, ISerializerExtension => Apply(@this, Support<T>.NewOrSingleton);

		public static IRootContext Apply<T>(this IRootContext @this, Func<T> create)
			where T : class, ISerializerExtension
		{
			if (!@this.Contains<T>())
			{
				@this.Add(create);
			}

			return @this;
		}

		public static T Add<T>(this IRootContext @this) where T : ISerializerExtension
			=> @this.Add(Support<T>.NewOrSingleton);

		public static T Add<T>(this IRootContext @this, Func<T> create) where T : ISerializerExtension
		{
			var result = create();
			@this.Add(result);
			return result;
		}

		public static IRootContext With<T>(this IRootContext @this, Action<T> configure)
			where T : class, ISerializerExtension
		{
			var extension = @this.With<T>();
			configure(extension);
			return @this;
		}

		public static T With<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? @this.Add<T>();

		public static IRootContext Extend(this IRootContext @this,
		                                  params ISerializerExtension[] extensions)
		{
			var items = With(@this, extensions)
				.ToList();
			@this.Clear();
			items.ForEach(@this.Add);
			return @this;
		}

		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions)
			        .ToArray();

		/*public static ITypeConfiguration Type(this IConfiguration @this, TypeInfo type) => @this.Get(type);*/

		public static IConfigurationContainer Configured<T>(this IConfigurationContainer @this)
			where T : class, IConfigurationProfile
			=> Support<T>.NewOrSingleton()
			             .Get(@this);

		public static ITypeConfiguration<T> ConfigureType<T>(this IConfigurationContainer @this) => @this.Type<T>();

		public static ITypeConfiguration<T> Type<T>(this IConfigurationContainer @this)
			=> @this.Root.Types.Get(Support<T>.Key)
			        .AsValid<TypeConfiguration<T>>();

		public static IConfigurationContainer Type<T>(this IConfigurationContainer @this,
		                                              Action<ITypeConfiguration<T>> configure)
		{
			var result = @this.Type<T>();
			configure(result);
			return @this;
		}

		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, TypeInfo type) =>
			@this.Root.Types.Get(type);

		public static IMemberConfiguration<T, TMember> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                  Expression<Func<T, TMember>> member) =>
			((IInternalTypeConfiguration)@this).Member(member.GetMemberInfo())
			                                   .AsValid<MemberConfiguration<T, TMember>>();

		public static IMemberConfiguration<T, TMember> Name<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                string name)
		{
			@this.AsInternal()
			     .Name(name);
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Order<T, TMember>(
			this IMemberConfiguration<T, TMember> @this, int order)
		{
			@this.AsInternal()
			     .Order(order);
			return @this;
		}

		public static ITypeConfiguration<T> Name<T>(this ITypeConfiguration<T> @this, string name)
		{
			((IInternalTypeConfiguration)@this).Name(name);
			return @this;
		}

		[Obsolete("This method has been replaced by MemberBy.")]
		public static ITypeConfiguration<T> Member<T>(this ITypeConfiguration<T> @this, MemberInfo member)
		{
			((IInternalTypeConfiguration)@this).Member(member);
			return @this;
		}

		public static IMemberConfiguration MemberBy<T>(this ITypeConfiguration<T> @this, MemberInfo member)
			=> ((IInternalTypeConfiguration)@this).Member(member);

		public static IMemberConfiguration<T, TMember> MemberBy<T, TMember>(
			this ITypeConfiguration<T> @this, MemberInfo<TMember> member)
			=> @this.MemberBy(member.Get())
			        .AsValid<MemberConfiguration<T, TMember>>();

		public static MemberInfo<T> As<T>(this MemberInfo @this) => new MemberInfo<T>(@this);

		internal static IMemberConfiguration Member(this ITypeConfiguration @this, string member)
		{
			var metadata = @this.Get()
			                    .GetMember(member)
			                    .SingleOrDefault();
			var result = metadata != null ? ((IInternalTypeConfiguration)@this).Member(metadata) : null;
			return result;
		}

		public static ITypeConfiguration<T> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                       Expression<Func<T, TMember>> member,
		                                                       Action<IMemberConfiguration<T, TMember>> configure)
		{
			configure(@this.Member(member));
			return @this;
		}

		public static IConfigurationContainer EnableReferences(this IConfigurationContainer @this)
		{
			@this.Root.EnableReferences();
			return @this;
		}

		public static IRootContext EnableReferences(this IRootContext @this)
		{
			@this.EnableRootInstances()
			     .With<ReferencesExtension>();
			return @this;
		}

		public static IConfigurationContainer EnableDeferredReferences(this IConfigurationContainer @this)
		{
			@this.Root.Extend(ReaderContextExtension.Default, DeferredReferencesExtension.Default);
			return @this;
		}

		public static ITypeConfiguration<T> EnableReferences<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                 Expression<Func<T, TMember>> member)
		{
			@this.Member(member)
			     .Identity();
			return @this;
		}

		public static IMemberConfiguration<T, TMember> Identity<T, TMember>(this IMemberConfiguration<T, TMember> @this)
		{
			@this.Attribute()
			     .Root
			     .EnableReferences()
			     .With<ReferencesExtension>()
			     .Assign(@this.Parent.AsValid<ITypeConfigurationContext>()
			                  .Get(), ((ISource<MemberInfo>)@this).Get());
			return @this;
		}

		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>()
			        .Whitelist;

		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>()
			        .Blacklist;
	}
}
