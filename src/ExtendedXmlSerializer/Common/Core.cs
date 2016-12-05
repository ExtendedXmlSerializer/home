// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExtendedXmlSerialization.Cache;

namespace ExtendedXmlSerialization.Common
{
    public class CompositeAlteration<T> : IAlteration<T>
    {
        private readonly IEnumerable<IAlteration<T>> _alterations;

        public CompositeAlteration(params IAlteration<T>[] alterations) : this(alterations.AsEnumerable()) {}

        public CompositeAlteration(IEnumerable<IAlteration<T>> alterations)
        {
            _alterations = alterations;
        }

        public T Get(T parameter) => _alterations.Aggregate(parameter, (current, alteration) => alteration.Get(current));
    }

    public class DecoratedAlteration<T> : IAlteration<T>
    {
        private readonly IAlteration<T> _alteration;
        public DecoratedAlteration(IAlteration<T> alteration)
        {
            _alteration = alteration;
        }

        public T Get(T parameter) => _alteration.Get(parameter);
    }

    public interface IAlteration<T> : IParameterizedSource<T, T> {}

    class Self<T> : IAlteration<T>
    {
        public static Self<T> Default { get; } = new Self<T>();
        Self() {}

        public T Get(T parameter) => parameter;
    }

    public class IsTypeSpecification<T> : ISpecification<object>
        {
            public static IsTypeSpecification<T> Default { get; } = new IsTypeSpecification<T>();
            IsTypeSpecification() {}

            public bool IsSatisfiedBy(object parameter) => parameter is T;
        }

    public class DelegatedSpecification<T> : ISpecification<T>
    {
        readonly Func<T, bool> _delegate;

        public DelegatedSpecification( Func<T, bool> @delegate )
        {
            _delegate = @delegate;
        }

        public bool IsSatisfiedBy( T parameter ) => _delegate.Invoke( parameter );
    }

    public abstract class SpecificationAdapterBase<T> : ISpecification<object>
    {
        public bool IsSatisfiedBy(object parameter) => parameter is T && IsSatisfiedBy((T)parameter);

        protected abstract bool IsSatisfiedBy(T parameter);
    }

    public class SpecificationAdapter<T> : SpecificationAdapterBase<T>
    {
        private readonly ISpecification<T> _specification;

        public SpecificationAdapter(ISpecification<T> specification)
        {
            _specification = specification;
        }

        protected override bool IsSatisfiedBy(T parameter) => _specification.IsSatisfiedBy(parameter);
    }

    

    public interface IParameterizedSource<in TParameter, out TResult>
    {
        TResult Get(TParameter parameter);
    }

    public interface IContent
    {
        object Value { get; }
    }

    public interface IProperty : IContent
    {
        string Name { get; }
    }

    abstract class PropertyBase : IProperty
    {
        protected PropertyBase(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }

    /*public static class SerializerExtensions
    {
        public static string Serialize(this ISerializer @this, object instance)
        {
            using (var stream = new MemoryStream())
            {
                @this.Serialize(stream, instance);
                stream.Seek(0, SeekOrigin.Begin);

                var result = new StreamReader(stream).ReadToEnd();
                return result;
            }
        }

        public static void Serialize(this ISerializer @this, Stream stream, object instance)
            => @this.Serialize(XmlWriter.Create(stream), instance);

        public static void Serialize(this ISerializer @this, XmlWriter xmlWriter, object instance)
        {
            using (var writer = new Writer(xmlWriter))
            {
                @this.Serialize(writer, instance);
            }
        }
    }*/

    class DefaultServiceProvider : IServiceProvider
    {
        public static DefaultServiceProvider Default { get; } = new DefaultServiceProvider();
        DefaultServiceProvider() {}

        public object GetService(Type serviceType) => null;
    }

    class DeferredInstruction : IInstruction
    {
        private readonly Lazy<IInstruction> _source;

        public DeferredInstruction(Func<IInstruction> source) : this(new Lazy<IInstruction>(source)) {}

        public DeferredInstruction(Lazy<IInstruction> source)
        {
            _source = source;
        }

        public void Execute(IServiceProvider services) => _source.Value?.Execute(services);
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

        private static object Callback(Type type) => type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }

    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy( T parameter );
    }

    /*class DecoratedInstruction : IInstruction
    {
        private readonly IInstruction _instruction;
        public DecoratedInstruction(IInstruction instruction)
        {
            _instruction = instruction;
        }

        public virtual void Execute(IServiceProvider services) => _instruction.Execute(services);
    }*/

    class Arrays : WeakCache<object, Array>
    {
        readonly static Array Array = (object[]) Enumerable.Empty<object>();

        public static Arrays Default { get; } = new Arrays();
        Arrays() : base(key => null) {}

        public Array AsArray(object instance)
        {
            var items = Get(instance);
            if (items != null)
            {
                return items;
            }
            var result = Is(instance)
                ? (instance as Array ?? ((IEnumerable) instance).Cast<object>().ToArray())
                : Array;
            Default.Add(instance, result);
            return result;
        }

        public bool Is(object instance) => TypeDefinitionCache.GetDefinition(instance.GetType()).IsEnumerable;
    }


    static class Extensions
    {
        public static string NullIfEmpty(this string target) => string.IsNullOrEmpty(target) ? null : target;

        // ATTRIBUTION: http://stackoverflow.com/a/5461399/3602057
        public static bool IsAssignableFromGeneric(this Type @this, Type candidate)
        {
            var interfaceTypes = candidate.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == @this)
                    return true;
            }

            var typeInfo = candidate.GetTypeInfo();
            if (typeInfo.IsGenericType && candidate.GetGenericTypeDefinition() == @this)
                return true;

            Type baseType = typeInfo.BaseType;
            if (baseType == null) return false;

            return IsAssignableFromGeneric(@this, baseType);
        }

        
        // public static TResult Accept<TParameter, TResult>( this TResult @this, TParameter _ ) => @this;

        // public static void Property(this IWriting @this, IProperty property) => @this.Property(property.Name, @this.Serialize(property.Value));

        public static ISpecification<object> Adapt<T>(this ISpecification<T> @this) => new SpecificationAdapter<T>(@this);

        /*public static IEnumerable<TResult> SelectAssigned<TSource, TResult>( this IEnumerable<TSource> @this, Func<TSource, TResult> select ) => @this.Select( select ).Where(item => item != null);*/

        /*public static IEnumerable<T> AsEnumerable<T>(this ImmutableArray<T> @this) => @this.OfType<T>(); // Avoids (direct) boxing.
        public static IEnumerable<T> Immutable<T>(this IEnumerable<T> @this) => @this.ToImmutableArray().AsEnumerable();*/

        public static Type GetMemberType(this MemberInfo memberInfo) =>
            (memberInfo as MethodInfo)?.ReturnType ??
            (memberInfo as PropertyInfo)?.PropertyType ??
            (memberInfo as FieldInfo)?.FieldType ??
            (memberInfo as TypeInfo)?.AsType();

        public static bool IsWritable(this MemberInfo memberInfo) =>
            (memberInfo as PropertyInfo)?.CanWrite ??
            !(memberInfo as FieldInfo)?.IsInitOnly ?? false;
            

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

    class CompositeInstruction : IInstruction/*, IEnumerable<IInstruction>*/
    {
        private readonly IEnumerable<IInstruction> _instructions;

        // public CompositeInstruction(IEnumerable<IInstruction> instructions) : this(instructions.ToImmutableArray()) {}
        public CompositeInstruction(params IInstruction[] instructions) : this(instructions.AsEnumerable()) {}
        public CompositeInstruction(IEnumerable<IInstruction> instructions)
        {
            _instructions = instructions;
        }

        public virtual void Execute(IServiceProvider services)
        {
            foreach (var instruction in _instructions)
            {
                instruction.Execute(services);
            }
        }

        /*public IEnumerator<IInstruction> GetEnumerator() => _instructions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();*/
    }

    sealed class DelegatedDisposable : IDisposable
    {
        private readonly Action _callback;
        public DelegatedDisposable(Action callback)
        {
            _callback = callback;
        }

        public void Dispose() => _callback();
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
}
