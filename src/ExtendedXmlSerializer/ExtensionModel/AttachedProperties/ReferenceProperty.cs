using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	/// <summary>
	/// Represents a referenced-based attached property.
	/// </summary>
	/// <typeparam name="TType">The hosting type.</typeparam>
	/// <typeparam name="TValue">The property's value.</typeparam>
	public class ReferenceProperty<TType, TValue> : Property<TType, TValue> where TType : class where TValue : class
	{
		/// <inheritdoc />
		public ReferenceProperty(Expression<Func<IProperty>> source) : this(source, _ => default) {}

		/// <inheritdoc />
		public ReferenceProperty(Expression<Func<IProperty>> source,
		                         ConditionalWeakTable<TType, TValue>.CreateValueCallback defaultValue)
			: base(new ReferenceCache<TType, TValue>(defaultValue), source) {}
	}
}