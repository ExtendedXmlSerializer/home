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
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Model
{
    public interface IMember : IReader, IWriter
    {
        XName Name { get; }
    }

    class Member : IMember
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;
        private readonly Typed _memberType;
        private readonly Func<object, object> _getter;

        public Member(IReader reader, IWriter writer, XName name, Typed memberType, Func<object, object> getter)
        {
            Name = name;
            _reader = reader;
            _writer = writer;
            _memberType = memberType;
            _getter = getter;
        }

        public XName Name { get; }

        public void Write(XmlWriter writer, object instance) => _writer.Write(writer, _getter(instance));

        public object Read(XElement element, Typed? hint = null) => _reader.Read(element, hint ?? _memberType);
    }

    public interface IAssignableMember : IMember
    {
        void Set(object instance, object value);
    }

    class AssignableMember : Member, IAssignableMember
    {
        private readonly Action<object, object> _setter;

        public AssignableMember(IReader reader, IWriter writer, XName name, Typed memberType, Func<object, object> getter,
                                Action<object, object> setter) : base(reader, writer, name, memberType, getter)
        {
            _setter = setter;
        }

        public void Set(object instance, object value) => _setter(instance, value);
    }


    public interface IMembers : IParameterizedSource<XName, IMember>, IEnumerable<IMember> {}

    public interface IInstanceMembers : IParameterizedSource<TypeInfo, IMembers> {}

    class InstanceMembers : WeakCacheBase<TypeInfo, IMembers>, IInstanceMembers
    {
        public InstanceMembers(Func<ISelector> selector)
            : this(selector, AllNames.Default, MemberNameProvider.Default, GetterFactory.Default,
                   SetterFactory.Default) {}

        private readonly Func<ISelector> _selector; // TODO: Should make a dependency. Currently it causes recursion.
        private readonly INames _names;
        private readonly INameProvider _name;
        private readonly IGetterFactory _getter;
        private readonly ISetterFactory _setter;

        public InstanceMembers(Func<ISelector> selector, INames names, INameProvider name, IGetterFactory getter,
                               ISetterFactory setter)
        {
            _selector = selector;
            _names = names;
            _name = name;
            _getter = getter;
            _setter = setter;
        }

        protected override IMembers Create(TypeInfo parameter) =>
            new Members(CreateMembers(parameter).OrderBy(x => x.Sort).Select(x => x.Member));

        IEnumerable<SortedMember> CreateMembers(TypeInfo declaringType)
        {
            var selector = _selector();
            foreach (var property in declaringType.GetProperties())
            {
                var getMethod = property.GetGetMethod(true);
                if (property.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
                    !(!property.GetSetMethod(true)?.IsPublic ?? false) &&
                    property.GetIndexParameters().Length <= 0 &&
                    !property.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    var type = new Typed(property.PropertyType);
                    var reader = selector.Get(type);
                    if (reader != null)
                    {
                        yield return Create(selector, reader, declaringType, property, type, !property.CanWrite);
                    }
                    else
                    {
                        // TODO: Warning? Throw?
                    }
                }
            }

            foreach (var field in declaringType.GetFields())
            {
                var readOnly = field.IsInitOnly;
                if ((readOnly ? !field.IsLiteral : !field.IsStatic) &&
                    !field.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    var type = new Typed(field.FieldType);
                    var reader = selector.Get(type);
                    if (reader != null)
                    {
                        yield return Create(selector, reader, declaringType, field, type, readOnly);
                    }
                    else
                    {
                        // TODO: Warning? Throw?
                    }
                }
            }
        }

        private SortedMember Create(ISelector selector, IReader reader, TypeInfo type, MemberInfo metadata, Typed memberType,
                                    bool readOnly)
        {
            var name = XName.Get(_name.Get(metadata).LocalName, _names.Get(type).NamespaceName);
            var sort = new Sort(metadata.GetCustomAttribute<XmlElementAttribute>(false)?.Order,
                                metadata.MetadataToken);
            var getter = _getter.Get(metadata);


            var writer = new ElementWriter(name.Accept, new SelectingWriter(selector));

            var member = readOnly
                ? new Member(reader, writer, name, memberType, getter)
                : new AssignableMember(reader, writer, name, memberType, getter, _setter.Get(metadata));
            var result = new SortedMember(member, sort);
            return result;
        }

        struct SortedMember
        {
            public SortedMember(IMember member, Sort sort)
            {
                Member = member;
                Sort = sort;
            }

            public IMember Member { get; }
            public Sort Sort { get; }
        }
    }

    sealed class Members : IMembers
    {
        private readonly ImmutableArray<IMember> _items;
        private readonly IDictionary<XName, IMember> _lookup;

        public Members(IEnumerable<IMember> items) : this(items.ToImmutableArray()) {}
        public Members(ImmutableArray<IMember> items) : this(items, items.ToDictionary(x => x.Name)) {}

        public Members(ImmutableArray<IMember> items, IDictionary<XName, IMember> lookup)
        {
            _items = items;
            _lookup = lookup;
        }

        public IMember Get(XName parameter)
        {
            IMember result;
            return _lookup.TryGetValue(parameter, out result) ? result : null;
        }

        public IEnumerator<IMember> GetEnumerator() => ((IEnumerable<IMember>) _items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface ISetterFactory : IParameterizedSource<MemberInfo, Action<object, object>> {}

    class SetterFactory : ISetterFactory
    {
        public static SetterFactory Default { get; } = new SetterFactory();
        SetterFactory() {}

        public Action<object, object> Get(MemberInfo parameter) => Get(parameter.DeclaringType, parameter.Name);

        static Action<object, object> Get(Type type, string name)
        {
            // Object (type object) from witch the data are retrieved
            var itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            var itemCasted = type.GetTypeInfo().IsValueType
                ? Expression.Unbox(itemObject, type)
                : Expression.Convert(itemObject, type);
            // Property from casted object
            var property = Expression.PropertyOrField(itemCasted, name);

            // Secound parameter - value to set
            var value = Expression.Parameter(typeof(object), "value");

            // Because we use this function also for value type we need to add conversion to object
            var paramCasted = Expression.Convert(value, property.Type);

            // Assign value to property
            var assign = Expression.Assign(property, paramCasted);

            var lambda = Expression.Lambda<Action<object, object>>(assign, itemObject, value);

            var result = lambda.Compile();
            return result;
        }
    }


    public interface IGetterFactory : IParameterizedSource<MemberInfo, Func<object, object>> {}

    public class GetterFactory : IGetterFactory
    {
        public static GetterFactory Default { get; } = new GetterFactory();
        GetterFactory() {}

        public Func<object, object> Get(MemberInfo parameter) => Get(parameter.DeclaringType, parameter.Name);

        static Func<object, object> Get(Type type, string name)
        {
            // Object (type object) from witch the data are retrieved
            var itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            var itemCasted = Expression.Convert(itemObject, type);

            // Property from casted object
            var property = Expression.PropertyOrField(itemCasted, name);

            // Because we use this function also for value type we need to add conversion to object
            var conversion = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(conversion, itemObject);
            var result = lambda.Compile();
            return result;
        }
    }

    struct Sort : IComparable<Sort>
    {
        public Sort(int? assigned, int value)
        {
            Assigned = assigned;
            Value = value;
        }

        int? Assigned { get; }
        int Value { get; }

        public int CompareTo(Sort other)
        {
            var right = !other.Assigned.HasValue;
            if (!Assigned.HasValue && right)
            {
                return Value.CompareTo(other.Value);
            }
            var compare = Assigned.GetValueOrDefault(-1).CompareTo(other.Assigned.GetValueOrDefault(-1));
            var result = right ? -compare : compare;
            return result;
        }
    }

/*    sealed class SortComparer : IComparer<Sort>
    {
        public static SortComparer Default { get; } = new SortComparer();
        SortComparer() {}

        public int Compare(Sort x, Sort y)
        {
            var right = !y.Assigned.HasValue;
            if (!x.Assigned.HasValue && right)
            {
                return x.Value.CompareTo(y.Value);
            }
            var compare = x.Assigned.GetValueOrDefault(-1).CompareTo(y.Assigned.GetValueOrDefault(-1));
            var result = right ? -compare : compare;
            return result;
        }
    }
*/
}