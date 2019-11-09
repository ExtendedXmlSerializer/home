using System;
using System.Linq.Expressions;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.AttachedProperties
{
	public class StructureProperty<TType, TValue> : Property<TType, TValue> where TType : class where TValue : struct
	{
		public StructureProperty(Expression<Func<IProperty>> source) : this(source, _ => default) {}

		public StructureProperty(Expression<Func<IProperty>> source, Func<TType, TValue> defaultValue)
			: base(new StructureCache<TType, TValue>(defaultValue), source) {}
	}
}