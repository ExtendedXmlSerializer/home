using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace ExtendedXmlSerialization.Write
{
    /*public static class KnownAttributes
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
    }*/

    public static class SerializerExtensions
    {
        readonly private static IWriterExtension[] Empty = {};

        public static string Serialize(this ISerializer @this, object instance) 
            => Serialize(@this, instance, Empty);

        public static string Serialize(this ISerializer @this, object instance, params IWriterExtension[] extensions)
        {
            using (var stream = new MemoryStream())
            {
                @this.Serialize(stream, instance, extensions);
                stream.Seek(0, SeekOrigin.Begin);

                var result = new StreamReader(stream).ReadToEnd();
                return result;
            }
        }

        public static void Serialize(this ISerializer @this, Stream stream, object instance)
            => Serialize(@this, stream, instance, Empty);
        public static void Serialize(this ISerializer @this, Stream stream, object instance, params IWriterExtension[] extensions)
            => @this.Serialize(XmlWriter.Create(stream), instance, extensions);

        public static void Serialize(this ISerializer @this, XmlWriter xmlWriter, object instance)
            => Serialize(@this, xmlWriter, instance, Empty);
        public static void Serialize(this ISerializer @this, XmlWriter xmlWriter, object instance, params IWriterExtension[] extensions)
        {
            using (var writer = new Writer(xmlWriter/*, extensions.Length == 1 ? extensions[0] : new CompositeExtension(extensions)*/))
            {
                @this.Serialize(writer, instance);
            }
        }
    }

    /*class FixedInstruction : IInstructionSource
    {
        private readonly IInstructionSource _source;
        private readonly Type _definition;

        public FixedInstruction(IInstructionSource source, Type definition)
        {
            _source = source;
            _definition = definition;
        }

        public IInstruction Get(Type type) => Get();

        public IInstruction Get() => _source.Get(_definition);
    }

    class DeferredInstruction : IInstruction
    {
        private readonly Func<IInstruction> _source;
        public DeferredInstruction(Func<IInstruction> source)
        {
            _source = source;
        }

        public void Execute(IWriter writer, object instance)
        {
            var instruction = _source();
            instruction?.Execute(writer, instance);
        }
    }*/

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

    /*public class DefaultWriteExtensions : CompositeExtension
    {
        public DefaultWriteExtensions(IExtendedXmlSerializer serializer) : base(
            new ObjectReferencesExtension(serializer),
            new VersionExtension(serializer)/*,
            AttributePropertiesExtension.Default#1#
        ) {}
    }*/

    /*public class AttributePropertiesExtension : WritingExtensionBase
    {
        public static AttributePropertiesExtension Default { get; } = new AttributePropertiesExtension();
        AttributePropertiesExtension() {}

        public override bool Write(XmlWriter writer, object instance, string content, MemberInfo member = null)
        {
            var attribute = member?.GetCustomAttribute<XmlAttributeAttribute>();
            if (attribute != null)
            {
                writer.WriteStartAttribute(attribute.AttributeName ?? instance as string ?? member.Name);
                return true;
            }
            return false;
        }
    }*/

    /*public class CompositeExtension : IWriterExtension
    {
        private readonly IEnumerable<IWriterExtension> _extensions;

        public CompositeExtension(params IWriterExtension[] extensions) : this(extensions.Immutable()) {}

        public CompositeExtension(IEnumerable<IWriterExtension> extensions)
        {
            _extensions = extensions;
        }

        public void Started(IWriter writer)
        {
            foreach (var extension in _extensions)
            {
                extension.Started(writer);
            }
        }

        public void Finished(IWriter writer)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(writer);
            }
        }
        public bool Started(IWriter writer, object instance)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Started(writer, instance))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IWriter writer, object instance)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(writer, writer);
            }
        }
        public bool Started(IWriter writer, object instance, string content)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Started(writer, instance, content))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IWriter writer, object instance, string content)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(writer, writer, content);
            }
        }

        public bool Started(IWriter writer, object instance, string name, string content)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Started(writer, instance, name, content))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IWriter writer, object instance, string name, string content)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(writer, writer, name, content);
            }
        }
    }*/

	

    /*class DecoratedInstruction : IInstruction
    {
        private readonly IInstruction _instruction;
        public DecoratedInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public virtual void Execute(IServiceProvider services, object instance) => _instruction.Execute(services, instance);
    }*/

    public static class Extensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this ImmutableArray<T> @this) => @this.OfType<T>(); // Avoids boxing.
        public static IEnumerable<T> Immutable<T>(this IEnumerable<T> @this) => @this.ToImmutableArray().AsEnumerable();

		/*public static Type GetMemberType(this MemberInfo memberInfo) =>
			(memberInfo as MethodInfo)?.ReturnType ??
			(memberInfo as PropertyInfo)?.PropertyType ??
			(memberInfo as FieldInfo)?.FieldType ??
			(memberInfo as TypeInfo)?.AsType();*/

	    public static T To<T>(this object @this) => @this is T ? (T) @this : default(T);

	    public static T Get<T>(this IServiceProvider @this) => @this.GetService(typeof(T)).To<T>();
		public static T GetValid<T>( this IServiceProvider @this ) => @this.GetService( typeof(T) ).AsValid<T>();

		public static T AsValid<T>( this object @this, string message = null )
		{
			if ( !( @this is T ) )
			{
				throw new InvalidOperationException( message ?? $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}." );
			}

			return (T)@this;
		}
    }

	class CompositeInstruction : IInstruction
    {
        private readonly IEnumerable<IInstruction> _instructions;
        public CompositeInstruction(params IInstruction[] instructions) : this(instructions.Immutable()) {}
        public CompositeInstruction(IEnumerable<IInstruction> instructions)
        {
            _instructions = instructions;
        }

        public virtual void Execute(IServiceProvider services, object instance)
        {
            foreach (var instruction in _instructions)
            {
                instruction.Execute(services, instance);
            }
        }
    }

    /*public class DecoratedWriter : IWriter
	{
		private readonly IWriter _writer;
		public DecoratedWriter(IWriter writer)
		{
			_writer = writer;
		}

		public virtual void StartObject(string name) => _writer.StartObject(name);
		public virtual void EndObject() => _writer.EndObject();
		public virtual void Emit(string content) => _writer.Emit(content);
		public virtual void Member(string name, string content) => _writer.Member(name, content);
		public virtual void Dispose() => _writer.Dispose();
	}*/

	/*public class ExtendedWriter : IWriter
	{
		private readonly IWriter _writer;
		private readonly IWriterExtension _extension;
		private readonly object _instance;
		private readonly Lazy<IWriter> _writerSource;

		public ExtendedWriter(IWriter writer, IWriterExtension extension, object instance)
		{
			_writer = writer;
			_extension = extension;
			_instance = instance;
			_writerSource =  new Lazy<IWriter>(Start);
		}

		private IWriter Start()
		{
			_extension.Started(_writer);
			return _writer;
		}

		private IWriter Writer => _writerSource.Value;

		public void StartObject(string name)
		{
			if (_extension.Started(Writer, _instance))
			{
				Writer.StartObject(name);
			}
		}

		public void EndObject()
		{
			Writer.EndObject();
			_extension.Finished(Writer, _instance);
		}

		public void Emit(string content)
		{
			if (_extension.Started(Writer, _instance, content))
			{
				Writer.Emit(content);
			}
			_extension.Finished(Writer, _instance, content);
		}

		public void Member(string name, string content)
		{
			if (_extension.Started(Writer, _instance, name, content))
			{
				Writer.Emit(content);
			}
			_extension.Finished(Writer, _instance, name, content);
		}

		public void Dispose()
		{
			_extension.Finished(_writer);
			_writer.Dispose();
		}
	}*/

	public interface IInstruction
    {
        void Execute(IServiceProvider services, object instance);
    }
}
