using System;
using System.Collections.Immutable;
using System.Xml;
using ExtendedXmlSerialization.Conversion.Xml;
using ExtendedXmlSerialization.Conversion.Xml.Properties;
using ExtendedXmlSerialization.TypeModel;

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
		readonly ImmutableArray<IElement> _arguments;
		public StartGenericElement(string displayName, string @namespace, ImmutableArray<IElement> arguments) : base(displayName, @namespace)
		{
			_arguments = arguments;
		}

		public override void Emit(XmlWriter writer, object instance)
		{
			base.Emit(writer, instance);

		}
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
		readonly IElements _elements;
		readonly IXmlElement _property;
		readonly ITypeFormatter _formatter;

		public StartVariableTypedMember(string name, Type classification)
			: this(name, classification, Elements.Default, TypeProperty.Default, TypeFormatter.Default) {}

		public StartVariableTypedMember(string name, Type classification, IElements elements, IXmlElement property,
		                                ITypeFormatter formatter) : base(name)
		{
			_classification = classification;
			_elements = elements;
			_property = property;
			_formatter = formatter;
		}

		public override void Emit(XmlWriter writer, object instance)
		{
			base.Emit(writer, instance);

			/*var actual = _elements.Actual(_classification, instance);
			if (actual != null)
			{
				/*writer.WriteAttributeString(_property.DisplayName, _property.Namespace, _formatter.Get(actual.Classification));#1#
				writer.WriteStartAttribute(_property.DisplayName, _property.Namespace);
				writer.WriteQualifiedName(actual.DisplayName, actual.Namespace);
				writer.WriteEndAttribute();
			}*/
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