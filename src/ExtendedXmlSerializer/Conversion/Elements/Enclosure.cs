using System;
using System.Collections.Immutable;
using System.Xml;
using ExtendedXmlSerialization.Conversion.Xml.Properties;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Elements
{
	class StartElement : EmitterBase
	{
		readonly string _displayName;
		readonly string _ns;

		public StartElement(string displayName, string @namespace)
		{
			_displayName = displayName;
			_ns = @namespace;
		}

		public override void Emit(XmlWriter writer, object instance) => writer.WriteStartElement(_displayName, _ns);
	}

	class StartGenericElement : StartElement
	{
		readonly ImmutableArray<Type> _arguments;
		readonly ITypeArgumentsProperty _property;

		public StartGenericElement(string displayName, string @namespace, ImmutableArray<Type> arguments)
			: this(displayName, @namespace, arguments, TypeArgumentsProperty.Default)
		{}

		public StartGenericElement(string displayName, string @namespace, ImmutableArray<Type> arguments, ITypeArgumentsProperty property)
			: base(displayName, @namespace)
		{
			_arguments = arguments;
			_property = property;
		}

		public override void Emit(XmlWriter writer, object instance)
		{
			base.Emit(writer, instance);
			_property.Emit(writer, _arguments);
		}
	}

	abstract class EmitterBase<T> : EmitterBase
	{
		public override void Emit(XmlWriter writer, object instance) => Emit(writer, instance.AsValid<T>());

		public abstract void Emit(XmlWriter writer, T instance);
	}

	abstract class EmitterBase : IEmitter
	{
		public abstract void Emit(XmlWriter writer, object instance);
	}

	class FinishElement : IEmitter
	{
		public static FinishElement Default { get; } = new FinishElement();
		FinishElement() {}

		public void Emit(XmlWriter writer, object instance) => writer.WriteEndElement();
	}

	class StartVariableTypedMember : StartMember
	{
		readonly Type _classification;
		readonly ITypeProperty _property;

		public StartVariableTypedMember(string name, Type classification) : this(name, classification, TypeProperty.Default) {}

		public StartVariableTypedMember(string name, Type classification, ITypeProperty property) : base(name)
		{
			_classification = classification;
			_property = property;
		}

		public override void Emit(XmlWriter writer, object instance)
		{
			base.Emit(writer, instance);

			var type = instance.GetType();
			if (_classification != type)
			{
				_property.Emit(writer, type);
			}
		}
	}

	class StartMember : IEmitter
	{
		readonly string _name;

		public StartMember(string name)
		{
			_name = name;
		}

		public virtual void Emit(XmlWriter writer, object instance) => writer.WriteStartElement(_name);
	}

	abstract class EnclosureBase : DecoratedEmitter
	{
		protected EnclosureBase(IEmitter body) : base(body) {}

		public override void Emit(XmlWriter writer, object instance)
		{
			Start(writer, instance);
			base.Emit(writer, instance);
			Finish(writer, instance);
		}

		protected abstract void Start(XmlWriter writer, object instance);

		protected abstract void Finish(XmlWriter writer, object instance);
	}

	class Enclosure : EnclosureBase
	{
		readonly IEmitter _start;
		readonly IEmitter _finish;

		public Enclosure(IEmitter start, IEmitter body) : this(start, body, FinishElement.Default) {}

		public Enclosure(IEmitter start, IEmitter body, IEmitter finish) : base(body)
		{
			_start = start;
			_finish = finish;
		}

		protected override void Start(XmlWriter writer, object instance) => _start.Emit(writer, instance);

		protected override void Finish(XmlWriter writer, object instance) => _finish.Emit(writer, instance);
	}
}