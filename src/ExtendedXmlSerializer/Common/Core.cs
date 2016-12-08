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
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ExtendedXmlSerialization.Cache;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Common
{
    /*public enum ConditionMonitorState
    {
        None,
        Applying,
        Applied
    }
    public sealed class ConditionMonitor
    {
        public bool IsApplied => State > ConditionMonitorState.None;

        public ConditionMonitorState State { get; private set; }

        public void Reset() => State = ConditionMonitorState.None;

        public bool Apply() => ApplyIf( null );

        public bool Apply( Action action ) => ApplyIf( null, action );

        public bool ApplyIf( bool? condition, Action action = null )
        {
            switch ( State )
            {
                case ConditionMonitorState.None:
                    State = ConditionMonitorState.Applying;
                    var updated = condition.GetValueOrDefault( true );
                    if ( updated )
                    {
                        action?.Invoke();
                    }
                    State = updated ? ConditionMonitorState.Applied : ConditionMonitorState.None;
                    return updated;
            }
            return false;
        }
    }*/
    public class ExtensionEnabledInstruction : DecoratedInstruction
    {
        public ExtensionEnabledInstruction(IInstruction instruction) : base(instruction) {}

        protected override void OnExecute(IServiceProvider services)
        {
            var extension = services.Get<IExtension>();
            if (extension?.Starting(services) ?? true)
            {
                base.OnExecute(services);
            }
            extension?.Finished(services);
        }
    }
    /*public class AnySpecification<T> : ISpecification<T>
    {
        readonly ImmutableArray<ISpecification<T>> specifications;

        public AnySpecification( params ISpecification<T>[] specifications )
        {
            this.specifications = specifications.ToImmutableArray();
        }

        public bool IsSatisfiedBy( T parameter )
        {
            foreach ( var specification in specifications )
            {
                if ( specification.IsSatisfiedBy( parameter ) )
                {
                    return true;
                }
            }
            return false;
        }
    }*/

    public class ObjectIdGenerator
    {
        private static readonly int[] Sizes = new int[21]
                                              {
                                                  5, 11, 29, 47, 97, 197, 397, 797, 1597, 3203, 6421, 12853, 25717, 51437,
                                                  102877, 205759, 411527, 823117, 1646237, 3292489, 6584983
                                              };

        
        int _currentCount, _currentSize;
        long[] _ids;
        object[] _objs;

        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" /> class.</summary>
        public ObjectIdGenerator()
        {
            _currentCount = 1;
            _currentSize = Sizes[0];
            _ids = new long[_currentSize*4];
            _objs = new object[_currentSize*4];
        }

        private int FindElement(object obj, out bool found)
        {
            int hashCode = RuntimeHelpers.GetHashCode(obj);
            int num1 = 1 + (hashCode & int.MaxValue)%(_currentSize - 2);
            while (true)
            {
                int num2 = (hashCode & int.MaxValue)%_currentSize*4;
                for (int index = num2; index < num2 + 4; ++index)
                {
                    if (_objs[index] == null)
                    {
                        found = false;
                        return index;
                    }
                    if (_objs[index] == obj)
                    {
                        found = true;
                        return index;
                    }
                }
                hashCode += num1;
            }
        }

        /// <summary>Returns the ID for the specified object, generating a new ID if the specified object has not already been identified by the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" />.</summary>
        /// <returns>The object's ID is used for serialization. <paramref name="firstTime" /> is set to true if this is the first time the object has been identified; otherwise, it is set to false.</returns>
        /// <param name="obj">The object you want an ID for. </param>
        /// <param name="firstTime">true if <paramref name="obj" /> was not previously known to the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" />; otherwise, false. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="obj" /> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" /> has been asked to keep track of too many objects. </exception>
        public virtual long GetId(object obj, out bool firstTime)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", ("ArgumentNull_Obj"));
            bool found;
            int element = FindElement(obj, out found);
            long id;
            if (!found)
            {
                _objs[element] = obj;
                long[] ids = _ids;
                int index = element;
                int currentCount = _currentCount;
                _currentCount = currentCount + 1;
                long num = (long) currentCount;
                ids[index] = num;
                id = _ids[element];
                if (_currentCount > _currentSize*4/2)
                    Rehash();
            }
            else
                id = _ids[element];
            firstTime = !found;
            return id;
        }

        /// <summary>Determines whether an object has already been assigned an ID.</summary>
        /// <returns>The object ID of <paramref name="obj" /> if previously known to the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" />; otherwise, zero.</returns>
        /// <param name="obj">The object you are asking for. </param>
        /// <param name="firstTime">true if <paramref name="obj" /> was not previously known to the <see cref="T:System.Runtime.Serialization.ObjectIDGenerator" />; otherwise, false. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="obj" /> parameter is null. </exception>
        public virtual long HasId(object obj, out bool firstTime)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", ("ArgumentNull_Obj"));
            bool found;
            int element = FindElement(obj, out found);
            if (found)
            {
                firstTime = false;
                return _ids[element];
            }
            firstTime = true;
            return 0;
        }

        private void Rehash()
        {
            int index1 = 0;
            int currentSize = _currentSize;
            while (index1 < Sizes.Length && Sizes[index1] <= currentSize)
                ++index1;
            if (index1 == Sizes.Length)
                throw new SerializationException("Serialization_TooManyElements");
            _currentSize = Sizes[index1];
            long[] numArray = new long[_currentSize*4];
            object[] objArray = new object[_currentSize*4];
            long[] ids = _ids;
            object[] objs = _objs;
            _ids = numArray;
            _objs = objArray;
            for (int index2 = 0; index2 < objs.Length; ++index2)
            {
                if (objs[index2] != null)
                {
                    bool found;
                    int element = FindElement(objs[index2], out found);
                    _objs[element] = objs[index2];
                    _ids[element] = ids[index2];
                }
            }
        }
    }

/// <summary>
    /// Attribution: https://msdn.microsoft.com/en-us/library/system.runtime.serialization.objectmanager(v=vs.110).aspx
    /// </summary>
    public abstract class ObjectWalkerBase<TInput, TResult> : IEnumerable<TResult>, IEnumerator<TResult>
    {
        readonly private Stack<TInput> _remaining = new Stack<TInput>();

        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        protected ObjectWalkerBase(TInput root)
        {
            Schedule(root);
        }

        public IEnumerator<TResult> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Reset()
        {
            throw new NotSupportedException("Resetting the enumerator is not supported.");
        }

        public TResult Current { get; private set; }

        object IEnumerator.Current => Current;

        protected bool Schedule(TInput candidate)
        {
            if (candidate != null)
            {
                // Ask the ObjectIDManager if this object has been examined before.
                
                // If this object has been examined before, do not look at it again just return.
                var result = First(candidate);
                if (result)
                {
                    OnSchedule(candidate);
                }
                return result;
            }
            return false;
        }

        bool First(object candidate)
        {
            bool firstOccurrence;
            _generator.GetId(candidate, out firstOccurrence);
            return firstOccurrence;
        }

        protected abstract TResult Select(TInput input);

        private void OnSchedule(TInput candidate) => _remaining.Push(candidate);

        // protected abstract bool ContainsAdditional(TResult parameter);
        /*protected abstract IEnumerable<TResult> Yield(TInput input);
        protected abstract IEnumerable<TResult> Next(TInput input);*/

        // Advance to the next item in the enumeration.
        public bool MoveNext()
        {
            // If there are no more items to enumerate, return false.
            if (_remaining.Count != 0)
            {
                var input = _remaining.Pop();
                Current = Select(input);


                /*foreach (var result in Yield(input))
                {
                    
                }*/

                /*var @continue = ContainsAdditional(current);
                
                if (@continue)
                {
                    foreach (var item in Yield(input))
                    {
                        Schedule(item);
                    }
                    // The object does have field, schedule the object's instance fields to be enumerated.
                    /*foreach (
                        FieldInfo fi in Current.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    )
                    {
                        Schedule(fi.GetValue(Current));
                    }#1#
                }*/
                return true;
            }

            return false;
        }

        public virtual void Dispose() {}
    }


    /*/// <summary>
    /// Attribution: https://msdn.microsoft.com/en-us/library/system.runtime.serialization.objectmanager(v=vs.110).aspx
    /// </summary>
    public abstract class ObjectWalkerBase<T> : IEnumerable<T>, IEnumerator<T>
    {
        // This stack contains the set of objects that will be enumerated.
        readonly private Stack<T> _remaining = new Stack<T>();

        // The ObjectIDGenerator ensures that each object is enumerated just once.
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        // Construct an ObjectWalker passing the root of the object graph.
        protected ObjectWalkerBase(params T[] seeding)
        {
            foreach (var root in seeding)
            {
                Schedule(root);
            }
        }

        // Return an enumerator so this class can be used with foreach.
        public IEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Resetting the enumerator is not supported.
        public void Reset()
        {
            throw new NotSupportedException("Resetting the enumerator is not supported.");
        }

        public T Current { get; private set; }

        // Return the enumeration's current object.
        object IEnumerator.Current => Current;

        // Walk the reference of the passed-in object.
        protected void Schedule(T candidate)
        {
            if (candidate != null)
            {
                // Ask the ObjectIDManager if this object has been examined before.
                bool firstOccurrence;
                _generator.GetId(candidate, out firstOccurrence);

                // If this object has been examined before, do not look at it again just return.
                if (firstOccurrence)
                {
                    /*var candidates = DetermineAdditionalFrom(candidate);
                    if (candidates != null)
                    {
                        foreach (var c in candidates)
                        {
                            Schedule(c);
                        }
                    }
                    else
                    {
                        
                    }#1#
                    OnSchedule(candidate);
                    /*if (candidate.GetType().IsArray)
                    {
                        // The object is an array, schedule each element of the array to be looked at.
                        foreach (var item in (Array) candidate)
                        {
                            Schedule(item);
                        }
                    }
                    else
                    {
                        // The object is not an array, schedule this object to be looked at.
                        _remaining.Push(candidate);
                    }#1#
                }
            }
        }

        private void OnSchedule(T candidate) => _remaining.Push(candidate);

        protected abstract bool Continue(T parameter);
        protected abstract IEnumerable<T> Yield(T candidate);

        // Advance to the next item in the enumeration.
        public bool MoveNext()
        {
            // If there are no more items to enumerate, return false.
            if (_remaining.Count != 0)
            {
                if (Continue(Current = _remaining.Pop()))
                {
                    foreach (var item in Yield(Current))
                    {
                        Schedule(item);
                    }
                    // The object does have field, schedule the object's instance fields to be enumerated.
                    /*foreach (
                        FieldInfo fi in Current.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    )
                    {
                        Schedule(fi.GetValue(Current));
                    }#1#
                }
                return true;
            }

            // Check if the object is a terminal object (has no fields that refer to other objects).
            return false;
        }

        // Returns true if the object has no data fields with information of interest.

        /*{
            Type t = data.GetType();
            return t.IsPrimitive || t.IsEnum || t.IsPointer || data is string;
        }#1#

        public virtual void Dispose() {}
    }*/

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

        public DelegatedSpecification(Func<T, bool> @delegate)
        {
            _delegate = @delegate;
        }

        public bool IsSatisfiedBy(T parameter) => _delegate.Invoke(parameter);
    }

    public class AlwaysSpecification<T> : ISpecification<T>
    {
        public static AlwaysSpecification<T> Default { get; } = new AlwaysSpecification<T>();
        AlwaysSpecification() {}

        public bool IsSatisfiedBy(T parameter) => true;
    }

    public abstract class SpecificationAdapterBase<T> : ISpecification<object>
    {
        public bool IsSatisfiedBy(object parameter) => parameter is T && IsSatisfiedBy((T) parameter);

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
public interface IAttachedProperties
    {
        void Attach(object instance, IProperty property);
        ICollection<IProperty> GetProperties(object instance);
    }

    class AttachedProperties : IAttachedProperties
    {
        public static AttachedProperties Default { get; } = new AttachedProperties();
        AttachedProperties() {}

        private readonly WeakCache<object, ICollection<IProperty>> 
            _properties = new WeakCache<object, ICollection<IProperty>>(_ => new OrderedSet<IProperty>());

        public void Attach(object instance, IProperty property) => _properties.Get(instance).Add(property);
        public ICollection<IProperty> GetProperties(object instance) => _properties.Get(instance);
    }
    public interface IProperty : IElement
    {
        object Value { get; }
    }

    public class Element : Namespace, IElement
    {
        public Element(INamespace @namespace, string name) : base(@namespace?.Prefix, @namespace?.Identifier)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public abstract class PropertyBase : Element, IProperty
    {
        protected PropertyBase(INamespace @namespace, string name, object value) : base(@namespace, name)
        {
            Value = value;
        }

        public object Value { get; }
    }

    /*public interface IContent
    {
        object Value { get; }
    }

    public interface IProperty : IContent
    {
        string Name { get; }
    }*/

    /*public abstract class PropertyBase : IProperty
    {
        protected PropertyBase(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }*/

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

    /*class DeferredInstruction : IInstruction
    {
        private readonly Lazy<IInstruction> _source;

        public DeferredInstruction(Func<IInstruction> source) : this(new Lazy<IInstruction>(source)) {}

        public DeferredInstruction(Lazy<IInstruction> source)
        {
            _source = source;
        }

        public void Execute(IServiceProvider services) => _source.Value?.Execute(services);
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

        private static object Callback(Type type) => type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }

    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T parameter);
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
        public static IEnumerable<T> Append<T>( this T @this, params T[] second ) => @this.Append( second.AsEnumerable() );
        public static IEnumerable<T> Append<T>( this T @this, ImmutableArray<T> second ) => @this.Append_( second.ToArray() );
        public static IEnumerable<T> Append<T>( this T @this, IEnumerable<T> second ) => @this.Append_( second );

        static IEnumerable<T> Append_<T>( this T @this, IEnumerable<T> second )
        {
            yield return @this;
            foreach ( var element1 in second )
                yield return element1;
        }


        public static T[] Fixed<T>( this T @this, params T[] items ) => @this.Append( items ).ToArray();

        // public static IEnumerable<T> Append<T>( this ImmutableArray<T> @this, params T[] items ) => @this.Concat( items );
        public static IEnumerable<T> Append<T>( this IEnumerable<T> @this, params T[] items ) => @this.Concat( items );
        /*public static IEnumerable<T> Append<T>( this IEnumerable<T> @this, T element )
        {
            foreach ( var element1 in @this )
                yield return element1;
            yield return element;
        }*/

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


        public static TResult Accept<TParameter, TResult>(this TResult @this, TParameter _) => @this;

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

        public static T Get<T>(this IServiceProvider @this) => @this is T ? (T) @this : @this.GetService(typeof(T)).To<T>();
        public static T GetValid<T>(this IServiceProvider @this) => @this.GetService(typeof(T)).AsValid<T>();

        public static T AsValid<T>(this object @this, string message = null)
        {
            if (!(@this is T))
            {
                throw new InvalidOperationException(message ?? $"'{@this.GetType().FullName}' is not of type {typeof(T).FullName}.");
            }

            return (T) @this;
        }
    }

    class CompositeInstruction : IInstruction /*, IEnumerable<IInstruction>*/
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
