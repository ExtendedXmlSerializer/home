using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	public abstract class ContextBase : IElementContext
	{
		protected ContextBase(TypeInfo classification)
		{
			Classification = classification;
		}

		public abstract void Emit(IEmitter emitter, object instance);
		public TypeInfo Classification { get; }
		public abstract object Yield(IYielder yielder);
	}

	public abstract class ContextBase<T> : ContextBase
	{
		protected ContextBase(TypeInfo classification) : base(classification) {}

		public override void Emit(IEmitter emitter, object instance) => Emit(emitter, (T) instance);

		public abstract void Emit(IEmitter emitter, T instance);
	}
}