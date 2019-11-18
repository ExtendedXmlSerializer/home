using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Linq.Expressions;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	/// <summary>
	/// Represents a structure-based attached property.
	/// </summary>
	/// <typeparam name="TType">The hosting type.</typeparam>
	/// <typeparam name="TValue">The property's value.</typeparam>
	public class StructureProperty<TType, TValue> : Property<TType, TValue> where TType : class where TValue : struct
	{
		/// <inheritdoc />
		public StructureProperty(Expression<Func<IProperty>> source) : this(source, _ => default) {}

		/// <inheritdoc />
		public StructureProperty(Expression<Func<IProperty>> source, Func<TType, TValue> defaultValue)
			: base(new StructureCache<TType, TValue>(defaultValue), source) {}
	}
}