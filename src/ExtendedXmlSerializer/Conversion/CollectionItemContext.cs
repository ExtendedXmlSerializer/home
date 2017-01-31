using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	class CollectionItemContext : NamedContext
	{
		readonly IContexts _contexts;

		public CollectionItemContext(IContexts contexts, IName elementName)
			: this(contexts, elementName, contexts.Get(elementName.Classification)) {}

		public CollectionItemContext(IContexts contexts, IName elementName, IElementContext body) : base(elementName, body)
		{
			_contexts = contexts;
		}

		public override void Emit(IEmitter emitter, object instance)
		{
			var type = instance.GetType();
			var actual = type.GetTypeInfo();
			if (Equals(actual, Name.Classification))
			{
				base.Emit(emitter, instance);
			}
			else
			{
				_contexts.Get(actual).Emit(emitter, instance);
			}
		}
	}
}