using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	public class DecoratedContext : ContextBase
	{
		readonly IElementContext _context;

		/*public DecoratedContext(IElementContext context) : this(context, context.Classification) {}*/

		public DecoratedContext(IElementContext context, TypeInfo classification) : base(classification)
		{
			_context = context;
		}

		public override void Emit(IEmitter emitter, object instance) => _context.Emit(emitter, instance);
		public override object Yield(IYielder yielder) => _context.Yield(yielder);
	}
}