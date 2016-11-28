using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.Write
{
	class PrimitiveInstructionProvider : IInstructionProvider
	{
		public static PrimitiveInstructionProvider Default { get; } = new PrimitiveInstructionProvider();
		PrimitiveInstructionProvider() {}

		public IInstruction Get(TypeDefinition definition) => new EmitContentInstruction(definition);
	}

	class DictionaryInstructionProvider : IInstructionProvider
	{
		private readonly IInstructionProvider _provider;
		
		public DictionaryInstructionProvider(IInstructionProvider provider)
		{
			_provider = provider;
		}

		public IInstruction Get(TypeDefinition definition)
		{
			var arguments = definition.GenericArguments;
			if (arguments.Length != 2)
			{
				throw new InvalidOperationException(
					      $"Attempted to write type '{definition.Type}' as a dictionary, but it does not have enough generic arguments.");
			}
			var keyDefinition = TypeDefinitionCache.GetDefinition(arguments[0]);
			var keys = new EncloseContentInstruction(ExtendedXmlSerializer.Key, _provider.Get(keyDefinition));

			var valueDefinition = TypeDefinitionCache.GetDefinition(arguments[1]);
			var values = new EncloseContentInstruction(ExtendedXmlSerializer.Value, _provider.Get(valueDefinition));
			var result = new EmitDictionaryInstruction(keys, values);
			return result;
		}
	}

	class EmitDictionaryInstruction : InstructionBase<IDictionary>
	{
		private readonly IInstruction _keys;
		private readonly IInstruction _values;

		public EmitDictionaryInstruction(IInstruction keys, IInstruction values)
		{
			_keys = keys;
			_values = values;
		}

		protected override void Execute(IWriter writer, IDictionary instance)
		{
			foreach (DictionaryEntry item in instance)
			{
				_keys.Execute(writer, item.Key);
				_values.Execute(writer, item.Value);
			}
		}
	}

	class EnumerableInstructionProvider : IInstructionProvider
	{
		private readonly IInstructionProvider _provider;

		public EnumerableInstructionProvider(IInstructionProvider provider)
		{
			_provider = provider;
		}

		public IInstruction Get(TypeDefinition definition)
		{
			var elementType = ElementTypeLocator.Default.Locate(definition.Type);
			var instructions = _provider.Get(TypeDefinitionCache.GetDefinition(elementType));
			var result = new EmitEnumerableInstruction(instructions);
			return result;
		}
	}

	abstract class InstructionBase<T> : IInstruction
	{
		protected abstract void Execute(IWriter writer, T instance);

		public void Execute(IWriter writer, object instance)
		{
			if (instance is T)
			{
				Execute(writer, (T) instance);
				return;
			}
			throw new InvalidOperationException(
				      $"Expected an instance of type '{typeof(T)}' but got an instance of '{instance.GetType()}'");
		}
	}

	class EmitEnumerableInstruction : InstructionBase<IEnumerable>
	{
		private readonly IInstruction _instruction;

		public EmitEnumerableInstruction(IInstruction instruction)
		{
			_instruction = instruction;
		}

		protected override void Execute(IWriter writer, IEnumerable instance)
		{
			var items = instance as Array ?? instance.Cast<object>().ToArray();
			foreach (var item in items)
			{
				_instruction.Execute(writer, item);
			}
		}
	}

	class EmitTypeInstruction : IInstruction
	{
		readonly TypeDefinition _definition;

		public EmitTypeInstruction(TypeDefinition definition)
		{
			_definition = definition;
		}

		public void Execute(IWriter writer, object instance)
		{
			if (instance.GetType() != _definition.Type)
			{
				writer.Write(KnownAttributes.Type, _definition.FullName);
			}
		}
	}

	class ConditionalInstructionProvider : IInstructionProvider
	{
		private readonly Func<TypeDefinition, bool> _specification;
		private readonly IInstructionProvider _inner;

		public ConditionalInstructionProvider(Func<TypeDefinition, bool> specification, IInstructionProvider inner)
		{
			_specification = specification;
			_inner = inner;
		}

		public IInstruction Get(TypeDefinition definition) => _specification(definition) ? _inner.Get(definition) : null;
	}

	public interface IWriting
	{
		void Write(object instance);
	}

	public interface IWriter : IDisposable
	{
		void Start(object instance, object context = null);
		void Write(object instance, object content, object context = null);
		void Finish(object instance);
	}

	public class EmptyExtension : WriteExtensionBase
	{
		public static EmptyExtension Default { get; } = new EmptyExtension();
		EmptyExtension() {}
	}

	public static class KnownAttributes
	{
		public static KnownAttribute Type { get; } = new KnownAttribute(ExtendedXmlSerializer.Type);
	}

	public class KnownAttribute
	{
		private readonly string _name;

		public KnownAttribute(string name)
		{
			_name = name;
		}

		public override string ToString() => _name;
	}

	public class Writer : IWriter
	{
		private readonly XmlWriter _writer;
		private readonly IWriteExtension _extension;

		public Writer(XmlWriter writer) : this(writer, EmptyExtension.Default) {}

		public Writer(XmlWriter writer, IWriteExtension extension)
		{
			_writer = writer;
			_extension = extension;
		}

		public void Start(object instance, object context = null)
		{
			switch (_writer.WriteState)
			{
			case WriteState.Start:
				_extension.Started(_writer);
				break;
			}

			var property = context as PropertyInfo;
			var text = instance as string ?? property?.Name ?? instance.ToString();
			var content = _extension.DetermineContent(_writer, instance, text, property);
			if (!_extension.Write(_writer, instance, content, property))
			{
				_writer.WriteStartElement(content);
			}
		}

		public void Write(object instance, object content, object context = null)
		{
			var property = context as PropertyInfo;
			var type = instance as KnownAttribute;
			var value = content as string;
			if (type != null && value != null)
			{
				_writer.WriteAttributeString(type.ToString(), value);
			}
			else
			{
				var coerced =  value ?? instance as string ?? content?.ToString() ?? instance.ToString();
				var text = _extension.DetermineContent(_writer, instance, coerced, property);
				if (!_extension.Write(_writer, instance, text, property))
				{
					_writer.WriteString(text);
				}
			}
			
			_extension.Wrote(_writer, instance, property);
		}

		public void Finish(object instance)
		{
			switch (_writer.WriteState)
			{
				case WriteState.Content:
					_writer.WriteEndElement();
					break;
				case WriteState.Attribute:
					_writer.WriteEndAttribute();
					break;
			}
			_extension.Wrote(_writer, instance);
		}

		public void Dispose()
		{
			_extension.Finished(_writer);
			_writer.Dispose();
		}
	}

	public class Writing : IWriting
	{
		public Writing(IWriter writer) : this(writer, RootInstructionProviderFactory.Default.Create()) {}

		private readonly IWriter _writer;
		private readonly IInstructionProvider _root;

		Writing(IWriter writer, IInstructionProvider root)
		{
			_writer = writer;
			_root = root;
		}

		public void Write(object instance)
		{
			var definition = TypeDefinitionCache.GetDefinition(instance.GetType());
			var instruction = _root.Get(definition);
			instruction.Execute(_writer, instance);
		}
	}

	class RootInstructionProviderFactory
	{
		public static RootInstructionProviderFactory Default { get; } = new RootInstructionProviderFactory();
		RootInstructionProviderFactory() {}

		public IInstructionProvider Create()
		{
			var collection = new List<IInstructionProvider>();
			var composite = new CachedInstructionProvider(new FirstAssignedInstructionProvider(collection));
			collection.AddRange(new IInstructionProvider[]
			                    {
				                    new ConditionalInstructionProvider(definition => definition.IsPrimitive, PrimitiveInstructionProvider.Default),
				                    new ConditionalInstructionProvider(definition => definition.IsDictionary, new DictionaryInstructionProvider(composite)),
				                    new ConditionalInstructionProvider(definition => definition.IsArray || definition.IsEnumerable,
				                                          new EnumerableInstructionProvider(composite)),
				                    new ConditionalInstructionProvider(definition => definition.IsObjectToSerialize, new ObjectInstructions(composite))
			                    });
			var result = new CachedInstructionProvider(new RootInstructionProvider(composite));
			return result;
		}
	}

	class RootInstructionProvider : IInstructionProvider
	{
		private readonly IInstructionProvider _inner;

		public RootInstructionProvider(IInstructionProvider inner)
		{
			_inner = inner;
		}

		public IInstruction Get(TypeDefinition definition)
		{
			var content = _inner.Get(definition);
			if (content == null)
			{
				throw new InvalidOperationException($"Could not find write instructions for type '{definition.Type}'");
			}
			var result = new EncloseContentInstruction(definition.Name, content);
			return result;
		}
	}

	class CachedInstructionProvider : IInstructionProvider
	{
		private readonly IInstructionProvider _inner;

		private readonly ConditionalWeakTable<TypeDefinition, IInstruction> _cache =
			new ConditionalWeakTable<TypeDefinition, IInstruction>();

		private readonly ConditionalWeakTable<TypeDefinition, IInstruction>.CreateValueCallback _callback;

		public CachedInstructionProvider(IInstructionProvider inner) : this(inner.Get) {}

		public CachedInstructionProvider(ConditionalWeakTable<TypeDefinition, IInstruction>.CreateValueCallback callback)
		{
			_callback = callback;
		}

		public IInstruction Get(TypeDefinition definition) => _cache.GetValue(definition, _callback);
	}

	class FirstAssignedInstructionProvider : IInstructionProvider
	{
		readonly ICollection<IInstructionProvider> _providers;

		public FirstAssignedInstructionProvider(ICollection<IInstructionProvider> providers)
		{
			_providers = providers;
		}

		public IInstruction Get(TypeDefinition definition)
		{
			foreach (var provider in _providers)
			{
				var instruction = provider.Get(definition);
				if (instruction != null)
				{
					return instruction;
				}
			}
			return null;
		}
	}

	public class VersionExtension : WriteExtensionBase
	{
		private readonly IExtendedXmlSerializer _serializer;

		public VersionExtension(IExtendedXmlSerializer serializer)
		{
			_serializer = serializer;
		}

		protected override bool Write(XmlWriter writer, object instance)
		{
			var type = instance.GetType();
			var version = _serializer
				.SerializationToolsFactory?
				.GetConfiguration(type)
				.Version;

			if (version != null)
			{
				writer.WriteAttributeString(ExtendedXmlSerializer.Version,
				                            version.Value.ToString(CultureInfo.InvariantCulture));
			}
			return false;
		}
	}

	public class EncryptionExtension : WriteExtensionBase
	{
		private readonly IExtendedXmlSerializer _serializer;

		public EncryptionExtension(IExtendedXmlSerializer serializer)
		{
			_serializer = serializer;
		}

		public override string DetermineContent(XmlWriter writer, object instance, string current, PropertyInfo property = null)
		{
			var type = instance.GetType();
			var configuration = _serializer.SerializationToolsFactory?.GetConfiguration(type);

			if (configuration?.CheckPropertyEncryption(property.Name) ?? false)
			{
				var algorithm = _serializer.SerializationToolsFactory?.EncryptionAlgorithm;
				if (algorithm != null)
				{
					var result = algorithm.Encrypt(current);
					return result;
				}
			}
			return current;
		}
	}

	public class ObjectReferencesExtension : WriteExtensionBase
	{
		private readonly IDictionary<string, object>
			_references = new ConcurrentDictionary<string, object>();

		private readonly IExtendedXmlSerializer _serializer;

		public ObjectReferencesExtension(IExtendedXmlSerializer serializer)
		{
			_serializer = serializer;
		}

		public override void Started(XmlWriter writer) => _references.Clear();

		protected override bool Write(XmlWriter writer, object instance)
		{
			var type = instance.GetType();
			var configuration = _serializer.SerializationToolsFactory?.GetConfiguration(type);

			if (configuration?.IsObjectReference ?? false)
			{
				var objectId = configuration.GetObjectId(instance);
				var key = $"{type.FullName}{ExtendedXmlSerializer.Underscore}{objectId}";
				var result = _references.ContainsKey(key);
				var name = result ? ExtendedXmlSerializer.Ref : ExtendedXmlSerializer.Id;
				writer.WriteAttributeString(name, objectId);
				if (!result)
				{
					_references.Add(key, instance);
				}
				return result;
			}

			return false;
		}

		public override void Finished(XmlWriter writer) => _references.Clear();
	}

	public abstract class WriteExtensionBase : IWriteExtension
	{
		public virtual void Started(XmlWriter writer) {}

		public virtual string DetermineContent(XmlWriter writer, object instance, string current, PropertyInfo property = null) => current;

		public virtual bool Write(XmlWriter writer, object instance, string content, PropertyInfo property = null)
		{
			var result = property != null ? Write(writer, instance, property, content) : Write(writer, instance);
			return result;
		}
		
		protected virtual bool Write(XmlWriter writer, object instance) => false;

		protected virtual bool Write(XmlWriter writer, object instance, PropertyInfo property, string content) => false;

		public virtual void Wrote(XmlWriter writer, object instance, PropertyInfo property = null) {}

		public virtual void Finished(XmlWriter writer) {}
	}

	class ObjectInstructions : IInstructionProvider
	{
		readonly IInstructionProvider _provider;
		
		public ObjectInstructions(IInstructionProvider provider)
		{
			_provider = provider;
		}

		public IInstruction Get(TypeDefinition definition)
		{
			var type = new EmitTypeInstruction(definition);
			var result = new CompositeInstruction(new[] {type}.Concat(GetContentInstructions(definition)));
			//var result = new EncloseContentInstruction(definition.Name, content);
			return result;
		}

		IEnumerable<IInstruction> GetContentInstructions(TypeDefinition definition)
		{
			foreach (var property in definition.Properties)
			{
				var instruction = _provider.Get(property.TypeDefinition);
				if (instruction != null)
				{
					yield return new EmitPropertyInstruction(property, instruction);
				}
			}
		}
	}

	public class DefaultValues
	{
		readonly ConditionalWeakTable<Type, object> _cache = new ConditionalWeakTable<Type, object>();
		readonly private ConditionalWeakTable<Type, object>.CreateValueCallback _callback;

		public static DefaultValues Default { get; } = new DefaultValues();
		DefaultValues()
		{
			_callback = Callback;
		}

		public object Get(Type type) => _cache.GetValue(type, _callback);

		private object Callback(Type type) => type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
	}

	class EmitPropertyInstruction : EncloseContentInstruction
	{
		private readonly PropertieDefinition _property;
		private readonly object _defaultValue;

		public EmitPropertyInstruction(PropertieDefinition property, IInstruction content)
			: this(property, content, DefaultValues.Default.Get(property.TypeDefinition.Type))
		{}

		public EmitPropertyInstruction(PropertieDefinition property, IInstruction content, object defaultValue)
			: base(property.Name, content, property.PropertyInfo)
		{
			_property = property;
			_defaultValue = defaultValue;
		}

		public override void Execute(IWriter writer, object instance)
		{
			var value = _property.GetValue(instance);
			if (value != _defaultValue)
			{
				base.Execute(writer, value);
			}
		}
	}

	class EmitContentInstruction : IInstruction
	{
		readonly TypeDefinition _definition;

		public EmitContentInstruction(TypeDefinition definition)
		{
			_definition = definition;
		}

		public void Execute(IWriter writer, object instance)
		{
			var content = PrimitiveValueTools.SetPrimitiveValue(instance, _definition);
			writer.Write(instance, content);
		}
	}

	public class DefaultWriteExtensions : CompositeExtension
	{
		public DefaultWriteExtensions(IExtendedXmlSerializer serializer) : base(
			new ObjectReferencesExtension(serializer),
			new VersionExtension(serializer),
			new EncryptionExtension(serializer),
			AttributePropertiesExtension.Default
		) {}
	}

	public class AttributePropertiesExtension : WriteExtensionBase
	{
		public static AttributePropertiesExtension Default { get; } = new AttributePropertiesExtension();
		AttributePropertiesExtension() {}

		public override bool Write(XmlWriter writer, object instance, string content, PropertyInfo property = null)
		{
			var attribute = property?.GetCustomAttribute<XmlAttributeAttribute>();
			if (attribute != null)
			{
				writer.WriteStartAttribute(attribute.AttributeName ?? instance as string ?? property.Name);
				return true;
			}
			return false;
		}
	}

	public class CompositeExtension : IWriteExtension
	{
		private readonly ImmutableArray<IWriteExtension> _extensions;

		public CompositeExtension(params IWriteExtension[] extensions) : this(extensions.ToImmutableArray()) {}

		public CompositeExtension(ImmutableArray<IWriteExtension> extensions)
		{
			_extensions = extensions;
		}

		public void Started(XmlWriter writer)
		{
			foreach (var extension in _extensions)
			{
				extension.Started(writer);
			}
		}

		public string DetermineContent(XmlWriter writer, object instance, string current, PropertyInfo property = null) => 
			_extensions.Aggregate(current, (s, extension) => extension.DetermineContent(writer, instance, s, property));

		public bool Write(XmlWriter writer, object instance, string content, PropertyInfo property = null)
		{
			foreach (var extension in _extensions)
			{
				if (extension.Write(writer, instance, content, property))
				{
					return true;
				}
			}
			return false;
		}

		public void Wrote(XmlWriter writer, object instance, PropertyInfo property = null)
		{
			foreach (var extension in _extensions)
			{
				extension.Wrote(writer, instance, property);
			}
		}

		public void Finished(XmlWriter writer)
		{
			foreach (var extension in _extensions)
			{
				extension.Finished(writer);
			}
		}
	}

	/*class DecoratedInstruction : IInstruction
	{
		private readonly IInstruction _instruction;
		public DecoratedInstruction(IInstruction instruction)
		{
			_instruction = instruction;
		}

		public virtual void Execute(IWriter writer, object instance) => _instruction.Execute(writer, instance);
	}*/

	class CompositeInstruction : IInstruction
	{
		private readonly ImmutableArray<IInstruction> _inner;

		public CompositeInstruction(params IInstruction[] inner) : this(inner.AsEnumerable()) {}
		public CompositeInstruction(IEnumerable<IInstruction> content) : this(content.ToImmutableArray()) {}

		public CompositeInstruction(ImmutableArray<IInstruction> inner)
		{
			_inner = inner;
		}

		public virtual void Execute(IWriter writer, object instance)
		{
			foreach (var instruction in _inner)
			{
				instruction.Execute(writer, instance);
			}
		}
	}

	class EncloseContentInstruction : CompositeInstruction
	{
		public EncloseContentInstruction(string name, IInstruction content, object context = null)
			: base(new StartObjectInstruction(name, context), content, FinishObjectInstruction.Default) {}
	}

	sealed class StartObjectInstruction : IInstruction
	{
		readonly string _elementName;
		private readonly object _context;

		public StartObjectInstruction(string elementName, object context = null)
		{
			_elementName = elementName;
			_context = context;
		}

		public void Execute(IWriter writer, object instance) => writer.Start(_elementName, _context);
	}

	sealed class FinishObjectInstruction : IInstruction
	{
		public static FinishObjectInstruction Default { get; } = new FinishObjectInstruction();
		FinishObjectInstruction() {}

		public void Execute(IWriter writer, object instance) => writer.Finish(instance);
	}

	interface IInstructionProvider
	{
		IInstruction Get(TypeDefinition definition);
	}

	public interface IInstruction
	{
		void Execute(IWriter writer, object instance);
	}



	public interface IWriteExtension
	{
		void Started(XmlWriter writer);

		string DetermineContent(XmlWriter writer, object instance, string current, PropertyInfo property = null);

		bool Write(XmlWriter writer, object instance, string content, PropertyInfo property = null);
		void Wrote(XmlWriter writer, object instance, PropertyInfo property = null);

		void Finished(XmlWriter writer);
	}
}
