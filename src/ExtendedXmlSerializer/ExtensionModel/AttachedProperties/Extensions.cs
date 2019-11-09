using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public static class Extensions
	{
		public static IConfigurationContainer EnableAttachedProperties(this IConfigurationContainer @this,
		                                                               params IProperty[] properties)
			=> @this.EnableAttachedProperties(new HashSet<IProperty>(properties));

		public static IConfigurationContainer EnableAttachedProperties(this IConfigurationContainer @this,
		                                                               ICollection<IProperty> properties)
			=> EnableAttachedProperties(@this, properties, new HashSet<Type>());

		public static IConfigurationContainer EnableAttachedProperties(this IConfigurationContainer @this,
		                                                               ICollection<IProperty> properties,
		                                                               ICollection<Type> types)
			=> @this.Extend(new AttachedPropertiesExtension(new Registrations<IProperty>(properties, types)));

		public static TValue Get<TType, TValue>(this TType @this, Property<TType, TValue> property)
			=> property.Get(@this);

		public static void Set<TType, TValue>(this TType @this, Property<TType, TValue> property, TValue value)
			=> property.Assign(@this, value);

		public static IMemberConfiguration<TType, TValue> AttachedProperty<TType, TValue>(
			this IConfigurationContainer @this,
			Expression<Func<Property<TType, TValue>>> property)
		{
			var instance = property.Compile()
			                       .Invoke();
			@this.Root.With<AttachedPropertiesExtension>()
			     .Registrations.Instances.Add(instance);
			var subject = property.GetMemberInfo()
			                      .AsValid<PropertyInfo>();

			var type = @this.GetTypeConfiguration(subject.DeclaringType);
			var current = type.AsInternal()
			                  .Member(subject);

			var result = new AttachedPropertyConfiguration<TType, TValue>(current);
			return result;
		}

		public static IConfigurationContainer AttachedProperty<TType, TValue>(
			this IConfigurationContainer @this,
			Expression<Func<Property<TType, TValue>>> property,
			Action<IMemberConfiguration<TType, TValue>> configure)
		{
			configure(@this.AttachedProperty(property));
			return @this;
		}
	}
}