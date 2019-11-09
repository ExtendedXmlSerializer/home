using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public class ReferenceProperty<TType, TValue> : Property<TType, TValue> where TType : class where TValue : class
	{
		public ReferenceProperty(Expression<Func<IProperty>> source) : this(source, _ => default) {}

		public ReferenceProperty(Expression<Func<IProperty>> source,
		                         ConditionalWeakTable<TType, TValue>.CreateValueCallback defaultValue)
			: base(new ReferenceCache<TType, TValue>(defaultValue), source) {}
	}
}