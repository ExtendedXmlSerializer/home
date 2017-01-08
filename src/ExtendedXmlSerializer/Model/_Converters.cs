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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    public interface ITypes : IParameterizedSource<XElement, Type> {}

    /*public class HintedRootTypes : ITypes
    {
        private readonly Type _hint;
        private readonly ITypes _provider;

        public HintedRootTypes(Type hint) : this(hint, Types.Default) {}

        public HintedRootTypes(Type hint, ITypes provider)
        {
            _hint = hint;
            _provider = provider;
        }

        public Type Get(XElement parameter)
            => parameter.Document.Root == parameter ? _hint : _provider.Get(parameter);
    }*/

    public class Types : WeakCacheBase<XElement, Type>, ITypes
    {
        public static Types Default { get; } = new Types();
        Types() : this(TypeParser.Default, Identities.Default) {}

        private readonly ITypeParser _parser;
        private readonly IIdentities _identities;

        public Types(ITypeParser parser, IIdentities identities)
        {
            _parser = parser;
            _identities = identities;
        }

        protected override Type Create(XElement parameter)
            => _identities.Get(parameter.Name) ?? FromAttribute(parameter);

        private Type FromAttribute(XElement parameter)
        {
            var value = parameter.Attribute(ExtendedXmlSerializer.Type)?.Value;
            var result = value != null ? _parser.Get(value) : null;
            /*if (result == null)
            {
                throw new SerializationException($"Could not find Type information from provided value: {value}");
            }*/

            return result;
        }
    }

    public interface IIdentities : ISpecification<Type>, ISpecification<XName>,
                                   IParameterizedSource<Type, XName>, IParameterizedSource<XName, Type> {}

    public class Identities : IIdentities
    {
        public static Identities Default { get; } = new Identities();
        Identities() : this(string.Empty) {}

        private readonly IDictionary<Type, XName> _names;
        private readonly IDictionary<XName, Type> _types;

        public Identities(string namespaceName)
            : this(new Dictionary<Type, XName>
                   {
                       {typeof(bool), XName.Get("boolean", namespaceName)},
                       {typeof(char), XName.Get("char", namespaceName)},
                       {typeof(sbyte), XName.Get("byte", namespaceName)},
                       {typeof(byte), XName.Get("unsignedByte", namespaceName)},
                       {typeof(short), XName.Get("short", namespaceName)},
                       {typeof(ushort), XName.Get("unsignedShort", namespaceName)},
                       {typeof(int), XName.Get("int", namespaceName)},
                       {typeof(uint), XName.Get("unsignedInt", namespaceName)},
                       {typeof(long), XName.Get("long", namespaceName)},
                       {typeof(ulong), XName.Get("unsignedLong", namespaceName)},
                       {typeof(float), XName.Get("float", namespaceName)},
                       {typeof(double), XName.Get("double", namespaceName)},
                       {typeof(decimal), XName.Get("decimal", namespaceName)},
                       {typeof(DateTime), XName.Get("dateTime", namespaceName)},
                       {typeof(DateTimeOffset), XName.Get("dateTimeOffset", namespaceName)},
                       {typeof(string), XName.Get("string", namespaceName)},
                       {typeof(Guid), XName.Get("guid", namespaceName)},
                       {typeof(TimeSpan), XName.Get("TimeSpan", namespaceName)}
                   }) {}

        public Identities(IDictionary<Type, XName> names)
            : this(names, names.ToDictionary(x => x.Value, y => y.Key)) {}

        public Identities(IDictionary<Type, XName> names, IDictionary<XName, Type> types)
        {
            _names = names;
            _types = types;
        }

        public XName Get(Type parameter)
        {
            XName result;
            return _names.TryGetValue(parameter, out result) ? result : null;
        }

        public Type Get(XName parameter)
        {
            Type result;
            return _types.TryGetValue(parameter, out result) ? result : null;
        }

        public bool IsSatisfiedBy(Type parameter) => _names.ContainsKey(parameter);

        public bool IsSatisfiedBy(XName parameter) => _types.ContainsKey(parameter);
    }

    public interface INames : IParameterizedSource<TypeInfo, XName> {}

    public class AllNames : WeakCacheBase<TypeInfo, XName>, INames
    {
        public static AllNames Default { get; } = new AllNames();
        AllNames() : this(Identities.Default, TypeNames.Default) {}

        private readonly IIdentities _identities;
        private readonly INames _types;

        public AllNames(IIdentities identities, INames types)
        {
            _identities = identities;
            _types = types;
        }

        protected override XName Create(TypeInfo parameter)
            => _identities.Get(parameter.AsType()) ?? _types.Get(parameter);
    }

    public interface ITypeName : ISpecification<TypeInfo>, IParameterizedSource<TypeInfo, XName> {}

    class TypeName : ITypeName
    {
        private readonly ISpecification<TypeInfo> _specification;
        private readonly IParameterizedSource<TypeInfo, XName> _source;

        public TypeName(ISpecification<TypeInfo> specification, IParameterizedSource<TypeInfo, XName> source)
        {
            _specification = specification;
            _source = source;
        }

        public bool IsSatisfiedBy(TypeInfo parameter) => _specification.IsSatisfiedBy(parameter);

        public XName Get(TypeInfo parameter) => _source.Get(parameter);
    }

    public class TypeNames : INames
    {
        public static TypeNames Default { get; } = new TypeNames();

        TypeNames() : this(
            new TypeName(
                new AnySpecification<TypeInfo>(IsArraySpecification.Default,
                                               new AllSpecification<TypeInfo>(IsGenericTypeSpecification.Default,
                                                                              IsEnumerableTypeSpecification.Default)),
                LegacyEnumerableNameProvider.Default),
            new TypeName(IsActivatedTypeSpecification.Default, NameProvider.Default)) {}

        private readonly ITypeName[] _names;

        public TypeNames(params ITypeName[] names)
        {
            _names = names;
        }

        public XName Get(TypeInfo parameter)
        {
            foreach (var name in _names)
            {
                if (name.IsSatisfiedBy(parameter))
                {
                    return name.Get(parameter);
                }
            }
            return null;
        }
    }

    public interface IConverter : IReader, IWriter {}

    public interface IConversion : ICandidate<TypeInfo, IConverter> {}

    public interface IWriter
    {
        void Write(XmlWriter writer, object instance);
    }

    public class Converter : ConverterBase
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;

        public Converter(IReader reader, IWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public override object Read(XElement element, Typed? hint = null) => _reader.Read(element, hint);

        public override void Write(XmlWriter writer, object instance)
            => _writer.Write(writer, instance);
    }

    public class Conversion : FixedCandidate<TypeInfo, IConverter>, IConversion
    {
        public Conversion(ISpecification<TypeInfo> specification, IReader reader, IWriter writer) : this(specification, new Converter(reader, writer)) {}
        public Conversion(ISpecification<TypeInfo> specification, IConverter converter) : base(specification, converter) {}
    }

    public abstract class ConverterBase : IConverter
    {
        public abstract void Write(XmlWriter writer, object instance);
        public abstract object Read(XElement element, Typed? hint = null);
    }

    public interface ISelectors : IParameterizedSource<ITypes, ISelector> {}

    class LegacyPrimitives : IEnumerable<IConversion>
    {
        public static LegacyPrimitives Default { get; } = new LegacyPrimitives();
        LegacyPrimitives() {}

        public IEnumerator<IConversion> GetEnumerator()
        {
            yield return LegacyBooleanConversion.Default;
            yield return LegacyCharacterConversion.Default;
            yield return ByteConversion.Default;
            yield return UnsignedByteConversion.Default;
            yield return ShortConversion.Default;
            yield return UnsignedShortConversion.Default;
            yield return IntegerConversion.Default;
            yield return UnsignedIntegerConversion.Default;
            yield return LongConversion.Default;
            yield return UnsignedLongConversion.Default;
            yield return FloatConversion.Default;
            yield return DoubleConversion.Default;
            yield return DecimalConversion.Default;
            yield return EnumerationConversion.Default;
            yield return DateTimeConversion.Default;
            yield return DateTimeOffsetConversion.Default;
            yield return StringConversion.Default;
            yield return GuidConversion.Default;
            yield return TimeSpanConversion.Default;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Selectors : WeakCacheBase<ITypes, ISelector>, ISelectors
    {
        public static Selectors Default { get; } = new Selectors();
        Selectors() : this(LegacyPrimitives.Default) {}

        private readonly IEnumerable<IConversion> _primitives;

        public Selectors(IEnumerable<IConversion> primitives)
        {
            _primitives = primitives;
        }

        protected override ISelector Create(ITypes parameter)
        {
            var source = new Source(this, parameter);
            var singleton = new SingletonSource<ISelector>(source.Get);
            var converter = new SelectingConverter(singleton.Get);
            var converters =
                _primitives
                    .Concat(Yield(parameter, converter))
                    .ToArray();
            var result = new NullableAwareSelector(new Selector(converters));
            return result;
        }

        protected virtual IEnumerable<IConversion> Yield(ITypes parameter, IConverter converter)
        {
            yield return new LegacyDictionaryConversion(parameter, converter);
            yield return new LegacyArrayConversion(parameter, converter);
            yield return new LegacyEnumerableConversion(parameter, converter);
            yield return new LegacyInstanceConversion(parameter, converter);
        }

        sealed class Source : ISource<ISelector>
        {
            private readonly ISelectors _selectors;
            private readonly ITypes _parameter;

            public Source(ISelectors selectors, ITypes parameter)
            {
                _selectors = selectors;
                _parameter = parameter;
            }

            public ISelector Get() => _selectors.Get(_parameter);
        }
    }

    public class SelectingConverter : Converter
    {
        public static SelectingConverter Default { get; } = new SelectingConverter();
        SelectingConverter() : this(Selectors.Default.Get(Types.Default).Self) {}

        public SelectingConverter(Func<ISelector> selector) : base(new SelectingReader(selector), new SelectingWriter(selector)) {}
    }

    public interface ISelector : ISelector<TypeInfo, IConverter> {}

    public class Selector : Selector<TypeInfo, IConverter>, ISelector
    {
        public Selector(params IConversion[] candidates) : base(candidates) {}
    }

    public class NullableAwareSelector : ISelector
    {
        private readonly ISelector _selector;

        public NullableAwareSelector(ISelector selector)
        {
            _selector = selector;
        }

        public IConverter Get(TypeInfo parameter) => _selector.Get(parameter.AccountForNullable());
    }

    public class TimeSpanConversion : PrimitiveConversionBase<TimeSpan>
    {
        public static TimeSpanConversion Default { get; } = new TimeSpanConversion();
        TimeSpanConversion() : base(XmlConvert.ToString, XmlConvert.ToTimeSpan) {}
    }

    public class GuidConversion : PrimitiveConversionBase<Guid>
    {
        public static GuidConversion Default { get; } = new GuidConversion();
        GuidConversion() : base(XmlConvert.ToString, XmlConvert.ToGuid) {}
    }

    public class DateTimeOffsetConversion : PrimitiveConversionBase<DateTimeOffset>
    {
        public static DateTimeOffsetConversion Default { get; } = new DateTimeOffsetConversion();
        DateTimeOffsetConversion() : base(XmlConvert.ToString, XmlConvert.ToDateTimeOffset) {}
    }

    public class DateTimeConversion : PrimitiveConversionBase<DateTime>
    {
        public static DateTimeConversion Default { get; } = new DateTimeConversion();

        DateTimeConversion()
            : base(
                x => XmlConvert.ToString(x, XmlDateTimeSerializationMode.RoundtripKind),
                x => XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.RoundtripKind)) {}
    }

    public class DecimalConversion : PrimitiveConversionBase<decimal>
    {
        public static DecimalConversion Default { get; } = new DecimalConversion();
        DecimalConversion() : base(XmlConvert.ToString, XmlConvert.ToDecimal) {}
    }

    public class DoubleConversion : PrimitiveConversionBase<double>
    {
        public static DoubleConversion Default { get; } = new DoubleConversion();
        DoubleConversion() : base(XmlConvert.ToString, XmlConvert.ToDouble) {}
    }

    public class FloatConversion : PrimitiveConversionBase<float>
    {
        public static FloatConversion Default { get; } = new FloatConversion();
        FloatConversion() : base(XmlConvert.ToString, XmlConvert.ToSingle) {}
    }

    public class LongConversion : PrimitiveConversionBase<long>
    {
        public static LongConversion Default { get; } = new LongConversion();
        LongConversion() : base(XmlConvert.ToString, XmlConvert.ToInt64) {}
    }

    public class UnsignedLongConversion : PrimitiveConversionBase<ulong>
    {
        public static UnsignedLongConversion Default { get; } = new UnsignedLongConversion();
        UnsignedLongConversion() : base(XmlConvert.ToString, XmlConvert.ToUInt64) {}
    }

    public class ByteConversion : PrimitiveConversionBase<sbyte>
    {
        public static ByteConversion Default { get; } = new ByteConversion();
        ByteConversion() : base(XmlConvert.ToString, XmlConvert.ToSByte) {}
    }

    public class UnsignedByteConversion : PrimitiveConversionBase<byte>
    {
        public static UnsignedByteConversion Default { get; } = new UnsignedByteConversion();
        UnsignedByteConversion() : base(XmlConvert.ToString, XmlConvert.ToByte) {}
    }

    public class ShortConversion : PrimitiveConversionBase<short>
    {
        public static ShortConversion Default { get; } = new ShortConversion();
        ShortConversion() : base(XmlConvert.ToString, XmlConvert.ToInt16) {}
    }

    public class UnsignedShortConversion : PrimitiveConversionBase<ushort>
    {
        public static UnsignedShortConversion Default { get; } = new UnsignedShortConversion();
        UnsignedShortConversion() : base(XmlConvert.ToString, XmlConvert.ToUInt16) {}
    }

    public class IntegerConversion : PrimitiveConversionBase<int>
    {
        public static IntegerConversion Default { get; } = new IntegerConversion();
        IntegerConversion() : base(XmlConvert.ToString, XmlConvert.ToInt32) {}
    }

    public class UnsignedIntegerConversion : PrimitiveConversionBase<uint>
    {
        public static UnsignedIntegerConversion Default { get; } = new UnsignedIntegerConversion();
        UnsignedIntegerConversion() : base(XmlConvert.ToString, XmlConvert.ToUInt32) {}
    }

    public class BooleanConversion : PrimitiveConversionBase<bool>
    {
        public static BooleanConversion Default { get; } = new BooleanConversion();
        BooleanConversion() : base(XmlConvert.ToString, XmlConvert.ToBoolean) {}
    }

    public class LegacyBooleanConversion : PrimitiveConversionBase<bool>
    {
        public static LegacyBooleanConversion Default { get; } = new LegacyBooleanConversion();
        LegacyBooleanConversion() : base(x => x.ToString(), Convert.ToBoolean) {}
    }

    public class CharacterConversion : PrimitiveConversionBase<char>
    {
        public static CharacterConversion Default { get; } = new CharacterConversion();
        CharacterConversion() : base(XmlConvert.ToString, XmlConvert.ToChar) {}
    }

    public class LegacyCharacterConversion : PrimitiveConversionBase<char>
    {
        public static LegacyCharacterConversion Default { get; } = new LegacyCharacterConversion();
        LegacyCharacterConversion() : base(x => XmlConvert.ToString((ushort) x), s => (char) XmlConvert.ToUInt16(s)) {}
    }

    public class StringConversion : PrimitiveConversionBase<string>
    {
        readonly private static Func<string, string> Self = Self<string>.Default.Get;

        public static StringConversion Default { get; } = new StringConversion();
        StringConversion() : base(Self, Self) {}
    }

    class TypeEmittingWriter : DecoratedWriter
    {
        private readonly ISpecification<TypeInfo> _specification;

        public TypeEmittingWriter(IWriter writer) : this(AlwaysSpecification<TypeInfo>.Default, writer) {}

        public TypeEmittingWriter(ISpecification<TypeInfo> specification, IWriter writer) : base(writer)
        {
            _specification = specification;
        }

        public override void Write(XmlWriter writer, object instance)
        {
            var type = new Typed(instance.GetType());
            var tracker = Tracker.Default.Get(writer);
            if (_specification.IsSatisfiedBy(type) && !tracker.Contains(instance))
            {
                tracker.Add(instance);
                writer.WriteAttributeString(ExtendedXmlSerializer.Type,
                                            LegacyTypeFormatter.Default.Format(type));
            }

            base.Write(writer, instance);
        }

        sealed class Tracker : WeakCache<XmlWriter, HashSet<object>>
        {
            public static Tracker Default { get; } = new Tracker();
            Tracker() : base(_ => new HashSet<object>()) {}
        }
    }

    public class EnumerationConversion : PrimitiveConversionBase<Enum>
    {
        public static EnumerationConversion Default { get; } = new EnumerationConversion();

        EnumerationConversion()
            : base(IsAssignableSpecification<Enum>.Default, new ValueWriter<Enum>(x => x.ToString()), EnumReader.Default
            ) {}
    }


    sealed class ListAdapter : IList
    {
        private readonly object _instance;
        private readonly Action<object, object> _add;

        public ListAdapter(object instance, Action<object, object> add)
        {
            _instance = instance;
            _add = add;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        int ICollection.Count
        {
            get { throw new NotSupportedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotSupportedException(); }
        }

        int IList.Add(object value)
        {
            _add(_instance, value);
            return 0;
        }

        bool IList.Contains(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        bool IList.IsReadOnly
        {
            get { throw new NotSupportedException(); }
        }

        bool IList.IsFixedSize
        {
            get { throw new NotSupportedException(); }
        }
    }

    public class LegacyArrayConversion : Conversion
    {
        public LegacyArrayConversion(ITypes types, IConverter converter)
            : base(IsArraySpecification.Default, new ArrayReader(types, converter), new EnumerableBodyWriter(converter)) {}
    }

    public class LegacyEnumerableConversion : Conversion
    {
        public LegacyEnumerableConversion(ITypes types, IConverter converter)
            : base(
                IsEnumerableTypeSpecification.Default,
                new ListReader(types, converter),
                new EnumerableBodyWriter(converter)
            ) {}
    }

    public class LegacyDictionaryConversion : Conversion
    {
        public LegacyDictionaryConversion(ITypes types, IConverter converter)
            : base(
                IsAssignableSpecification<IDictionary>.Default, new DictionaryReader(types, converter),
                new DictionaryBodyWriter(converter)) {}
    }

    public class IsArraySpecification : ISpecification<TypeInfo>
    {
        public static IsArraySpecification Default { get; } = new IsArraySpecification();
        IsArraySpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsArray;
    }

    public class IsGenericTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsGenericTypeSpecification Default { get; } = new IsGenericTypeSpecification();
        IsGenericTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter) => parameter.IsGenericType;
    }

    /*public class IsEnumerableTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsEnumerableTypeSpecification Default { get; } = new IsEnumerableTypeSpecification();
        IsEnumerableTypeSpecification() : this(ElementTypeLocator.Default) {}

        private readonly IElementTypeLocator _locator;

        public IsEnumerableTypeSpecification(IElementTypeLocator locator)
        {
            _locator = locator;
        }

        public bool IsSatisfiedBy(TypeInfo parameter) => _locator.Locate(parameter.AsType()) != null;
    }*/

    public class IsEnumerableTypeSpecification : IsAssignableSpecification<IEnumerable>
    {
        public static new IsEnumerableTypeSpecification Default { get; } = new IsEnumerableTypeSpecification();
        IsEnumerableTypeSpecification()  {}
    }

    public static class Extensions
    {
        public static TypeInfo AccountForNullable(this TypeInfo @this)
            => Nullable.GetUnderlyingType(@this.AsType())?.GetTypeInfo() ?? @this;

        public static Type AccountForNullable(this Type @this) => Nullable.GetUnderlyingType(@this) ?? @this;

        public static T Activate<T>(this IActivators @this, Typed type) => (T) @this.Get(type).Invoke();
    }

    class InstanceBodyReader : ReaderBase
    {
        private readonly IInstanceMembers _members;
        private readonly ITypes _types;
        private readonly IActivators _activators;

        public InstanceBodyReader(IInstanceMembers members, ITypes types, IActivators activators)
        {
            _members = members;
            _types = types;
            _activators = activators;
        }

        public override object Read(XElement element, Typed? hint = null)
        {
            var type = hint ?? _types.Get(element);
            var result = type.HasValue ? Create(element, type.Value) : null;
            return result;
        }

        protected virtual object Create(XElement element, Typed type)
        {
            var result = _activators.Activate<object>(type);
            OnRead(element, result, type);
            return result;
        }

        protected virtual void OnRead(XElement element, object result, Typed type)
        {
            var members = _members.Get(type);
            foreach (var child in element.Elements())
            {
                var member = members.Get(child.Name);
                if (member != null)
                {
                    Apply(result, member, member.Read(child, _types.Get(child)));
                }
            }
        }

        protected virtual void Apply(object instance, IMember member, object value)
        {
            if (value != null)
            {
                var assignable = member as IAssignableMember;
                assignable?.Set(instance, value);
            }
        }
    }

    class InstanceBodyWriter : WriterBase
    {
        private readonly IInstanceMembers _members;

        public InstanceBodyWriter(IInstanceMembers members)
        {
            _members = members;
        }

        public override void Write(XmlWriter writer, object instance)
        {
            foreach (var member in _members.Get(instance.GetType().GetTypeInfo()))
            {
                member.Write(writer, instance);
            }
        }
    }

    public class LegacyInstanceConversion : Conversion
    {
        public LegacyInstanceConversion(ITypes types, IConverter converter)
            : this(IsActivatedTypeSpecification.Default, types, converter) {}

        protected LegacyInstanceConversion(ISpecification<TypeInfo> specification, ITypes types,
                                               IConverter converter)
            : this(
                specification, new InstanceMembers(converter, new EnumeratingReader(types, converter)), types,
                Activators.Default) {}

        public LegacyInstanceConversion(ISpecification<TypeInfo> specification, IInstanceMembers members,
                                            ITypes types,
                                            IActivators activators)
            : base(
                specification, new InstanceBodyReader(members, types, activators),
                new TypeEmittingWriter(new InstanceBodyWriter(members))) {}
    }


    public class IsActivatedTypeSpecification : ISpecification<TypeInfo>
    {
        public static IsActivatedTypeSpecification Default { get; } = new IsActivatedTypeSpecification();
        IsActivatedTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter)
            => parameter.IsValueType ||
               !parameter.IsAbstract && parameter.IsClass && parameter.GetConstructor(Type.EmptyTypes) != null;
    }

    public class EmitTypeSpecification : ISpecification<TypeInfo>
    {
        public static EmitTypeSpecification Default { get; } = new EmitTypeSpecification();
        EmitTypeSpecification() {}

        public bool IsSatisfiedBy(TypeInfo parameter)
            => !parameter.IsPrimitive &&
               !parameter.IsArray &&
               !typeof(IEnumerable).IsAssignableFrom(parameter.AsType()) &
               IsActivatedTypeSpecification.Default.IsSatisfiedBy(parameter);
    }

    public abstract class PrimitiveConversionBase<T> : Conversion
    {
        protected PrimitiveConversionBase(Func<T, string> serialize,
                                         Func<string, T> deserialize)
            : this(
                new ValueWriter<T>(serialize),
                new ValueReader<T>(deserialize)
            ) {}

        protected PrimitiveConversionBase(IWriter writer, IReader reader)
            : this(TypeEqualitySpecification<T>.Default, writer, reader) {}

        protected PrimitiveConversionBase(ISpecification<TypeInfo> specification, IWriter writer, IReader reader)
            : base(specification, new ValueValidatingReader(reader), new InstanceValidatingWriter(writer)) {}
    }
}