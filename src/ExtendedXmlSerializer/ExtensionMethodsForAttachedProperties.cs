using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.ExtensionModel.AttachedProperties;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Extension methods that assist or enable functionality found within the extension model namespace for attached
	/// properties (<see cref="ExtensionModel.AttachedProperties"/>).
	/// </summary>
	public static class ExtensionMethodsForAttachedProperties
	{
		/// <summary>
		/// Enables attached properties on the container with optional initial properties to register.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <param name="properties">The properties to register with the container.</param>
		/// <returns>The configured IConfigurationContainer.</returns>
		public static IConfigurationContainer EnableAttachedProperties(this IConfigurationContainer @this,
		                                                               params IProperty[] properties)
			=> @this.EnableAttachedProperties(new HashSet<IProperty>(properties));

		/// <summary>
		/// Enables attached properties on the container with the initial properties to register.
		/// </summary>
		/// <param name="this">The container to configure.</param>
		/// <param name="properties">The properties to register with the container.</param>
		/// <returns>The configured IConfigurationContainer.</returns>
		public static IConfigurationContainer EnableAttachedProperties(this IConfigurationContainer @this,
		                                                               ICollection<IProperty> properties)
			=> @this.Extend(new AttachedPropertiesExtension(new Registrations<IProperty>(properties)));

		/// <summary>
		/// Registers an attached property by expression.
		/// </summary>
		/// <typeparam name="TType">The type of object the attached property targets.</typeparam>
		/// <typeparam name="TValue">The type of the value the attached property provides.</typeparam>
		/// <param name="this">The container to configure.</param>
		/// <param name="property">The expression that resolves to an attached property.</param>
		/// <returns>The configured IMemberConfiguration that represents the attached property.</returns>
		public static IMemberConfiguration<TType, TValue> AttachedProperty<TType, TValue>(
			this IConfigurationContainer @this,
			Expression<Func<Property<TType, TValue>>> property)
		{
			var instance = property.Compile()();
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

		/// <summary>
		/// Given an instance, gets the value stored with the attached property.
		/// </summary>
		/// <typeparam name="TType">The instance type.</typeparam>
		/// <typeparam name="TValue">The stored value type.</typeparam>
		/// <param name="this">The instance to use to retrieve the value.</param>
		/// <param name="property">The property used to retrieve the value.</param>
		/// <returns>The value stored with the attached property.</returns>
		public static TValue Get<TType, TValue>(this TType @this, Property<TType, TValue> property)
			=> property.Get(@this);

		/// <summary>
		/// Given an instance and value, stores a value with the attached property.
		/// </summary>
		/// <typeparam name="TType">The instance type.</typeparam>
		/// <typeparam name="TValue">The stored value type.</typeparam>
		/// <param name="this">The instance with which to store the value.</param>
		/// <param name="property">The attached property to store the value.</param>
		/// <param name="value">The value to store.</param>
		public static void Set<TType, TValue>(this TType @this, Property<TType, TValue> property, TValue value)
			=> property.Assign(@this, value);

		#region Obsolete

		/// <exclude />
		[Obsolete("This method is deprecated and will be removed in a future release.")]
		public static IConfigurationContainer EnableAttachedProperties(this IConfigurationContainer @this,
		                                                               ICollection<IProperty> properties,
		                                                               ICollection<Type> types)
			=> @this.Extend(new AttachedPropertiesExtension(new Registrations<IProperty>(properties, types)));

		/// <summary>
		/// This method is not used and will be removed in a future release.
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="this"></param>
		/// <param name="property"></param>
		/// <param name="configure"></param>
		/// <returns></returns>
		[Obsolete("This method is not used and will be removed in a future release.")]
		public static IConfigurationContainer AttachedProperty<TType, TValue>(
			this IConfigurationContainer @this,
			Expression<Func<Property<TType, TValue>>> property,
			Action<IMemberConfiguration<TType, TValue>> configure)
			=> configure.Apply(@this.AttachedProperty(property)).Return(@this);

		#endregion
	}
}
