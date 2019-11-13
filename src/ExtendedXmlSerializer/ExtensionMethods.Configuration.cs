using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
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

		public static IRootContext Apply<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Apply(Support<T>.NewOrSingleton);

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
			=> @this.Apply(create()).AsValid<T>();

		public static IRootContext With<T>(this IRootContext @this, Action<T> configure)
			where T : class, ISerializerExtension
			=> configure.Apply(@this.With<T>()).Return(@this);

		public static T With<T>(this IRootContext @this) where T : class, ISerializerExtension
			=> @this.Find<T>() ?? @this.Add<T>();

		public static IRootContext Extend(this IRootContext @this, params ISerializerExtension[] extensions)
		{
			var items = With(@this, extensions).ToList();
			@this.Clear();
			items.ForEach(@this.Add);
			return @this;
		}

		public static ISerializerExtension[] With(this IEnumerable<ISerializerExtension> @this,
		                                          params ISerializerExtension[] extensions)
			=> @this.TypeZip(extensions).ToArray();

		public static IConfigurationContainer Configured<T>(this IConfigurationContainer @this)
			where T : class, IConfigurationProfile
			=> Support<T>.NewOrSingleton().Get(@this);

		public static ITypeConfiguration<T> Type<T>(this IConfigurationContainer @this)
			=> @this.Root.Types.Get(Support<T>.Key)
			        .AsValid<TypeConfiguration<T>>();

		public static IConfigurationContainer Type<T>(this IConfigurationContainer @this,
		                                              Action<ITypeConfiguration<T>> configure)
			=> configure.Apply(@this.Type<T>()).Return(@this);

		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, Type type)
			=> @this.GetTypeConfiguration(type.GetTypeInfo());

		public static ITypeConfiguration GetTypeConfiguration(this IContext @this, TypeInfo type)
			=> @this.Root.Types.Get(type);

		public static IMemberConfiguration<T, TMember> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                  Expression<Func<T, TMember>> member)
			=> @this.AsInternal().Member(member.GetMemberInfo()).AsValid<MemberConfiguration<T, TMember>>();

		public static IMemberConfiguration<T, TMember> Name<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                string name)
			=> @this.AsInternal().Name(name).Return(@this);

		public static IMemberConfiguration<T, TMember> Order<T, TMember>(this IMemberConfiguration<T, TMember> @this,
		                                                                 int order)
			=> @this.AsInternal().Order(order).Return(@this);

		public static ITypeConfiguration<T> Name<T>(this ITypeConfiguration<T> @this, string name)
			=> @this.AsInternal().Name(name).Return(@this);

		public static IMemberConfiguration MemberBy<T>(this ITypeConfiguration<T> @this, MemberInfo member)
			=> @this.AsInternal().Member(member);

		public static IMemberConfiguration<T, TMember> MemberBy<T, TMember>(
			this ITypeConfiguration<T> @this, MemberInfo<TMember> member)
			=> @this.MemberBy(member.Get()).AsValid<MemberConfiguration<T, TMember>>();

		public static MemberInfo<T> As<T>(this MemberInfo @this) => new MemberInfo<T>(@this);

		internal static IMemberConfiguration Member(this ITypeConfiguration @this, string member)
		{
			var metadata = @this.Get()
			                    .GetMember(member)
			                    .SingleOrDefault();
			var result = metadata != null ? @this.AsInternal().Member(metadata) : null;
			return result;
		}

		public static ITypeConfiguration<T> Member<T, TMember>(this ITypeConfiguration<T> @this,
		                                                       Expression<Func<T, TMember>> member,
		                                                       Action<IMemberConfiguration<T, TMember>> configure)
			=> configure.Apply(@this.Member(member)).Return(@this);

		public static IConfigurationContainer EnableReferences(this IConfigurationContainer @this)
			=> @this.Root.EnableReferences().Return(@this);

		public static IRootContext EnableReferences(this IRootContext @this)
			=> @this.EnableRootInstances().With<ReferencesExtension>().Return(@this);

		public static IConfigurationContainer EnableDeferredReferences(this IConfigurationContainer @this)
			=> @this.Root.Extend(ReaderContextExtension.Default, DeferredReferencesExtension.Default).Return(@this);

		public static ITypeConfiguration<T> EnableReferences<T, TMember>(this ITypeConfiguration<T> @this,
		                                                                 Expression<Func<T, TMember>> member)
			=> @this.Member(member).Identity().Return(@this);

		public static IMemberConfiguration<T, TMember> Identity<T, TMember>(this IMemberConfiguration<T, TMember> @this)
			=> @this.Attribute()
			        .Root.EnableReferences()
			        .With<ReferencesExtension>()
			        .Apply(@this.Parent.AsValid<ITypeConfigurationContext>().Get(), @this.GetMember())
			        .Return(@this);

		public static ICollection<TypeInfo> AllowedReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>().Whitelist;

		public static ICollection<TypeInfo> IgnoredReferenceTypes(this IConfigurationContainer @this)
			=> @this.Root.With<DefaultReferencesExtension>().Blacklist;

		#region Obsolete

		[Obsolete("This will be removed in a future release.  Use IConfigurationContainer.Type<T> instead.")]
		public static ITypeConfiguration<T> ConfigureType<T>(this IConfigurationContainer @this) => @this.Type<T>();

		[Obsolete("This method has been replaced by MemberBy.")]
		public static ITypeConfiguration<T> Member<T>(this ITypeConfiguration<T> @this, MemberInfo member)
		{
			((IInternalTypeConfiguration)@this).Member(member);
			return @this;
		}

		#endregion
	}
}