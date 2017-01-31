using System;
using System.Reflection;

namespace ExtendedXmlSerialization.Conversion
{
	class ValueContext<T> : ContextBase<T>
	{
		readonly static TypeInfo Type = typeof(T).GetTypeInfo();

		readonly Func<string, T> _deserialize;
		readonly Func<T, string> _serialize;

		public ValueContext(Func<string, T> deserialize, Func<T, string> serialize) : base(Type)
		{
			_deserialize = deserialize;
			_serialize = serialize;
		}

		public override void Emit(IEmitter emitter, T instance) => emitter.Write(_serialize(instance));

		public override object Yield(IYielder yielder) => _deserialize(yielder.Value());
	}
}