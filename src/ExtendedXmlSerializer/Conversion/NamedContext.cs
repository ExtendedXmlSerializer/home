using System.Reflection;
using ExtendedXmlSerialization.Conversion.Model.Names;

namespace ExtendedXmlSerialization.Conversion
{
	public class NamedContext : NamedContext<IName>
	{
		public NamedContext(IName name, IElementContext body) : base(name, body) {}
	}

	public class NamedContext<T> : DecoratedContext where T : IName
	{
		public NamedContext(T name, IElementContext body) : this(name, body, body.Classification) {}

		public NamedContext(T name, IElementContext body, TypeInfo classification) : base(body, classification)
		{
			Name = name;
		}

		protected T Name { get; }

		public override void Emit(IEmitter emitter, object instance)
		{
			using (emitter.Emit(Name))
			{
				base.Emit(emitter, instance);
			}
		}
	}

}