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
using System.Xml.Linq;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Model
{
    public interface IMember
    {
        XName Name { get; }

        MemberInfo Info { get; }

        IType Owner { get; }

        Sort Sort { get; }

        object Get(object instance);
    }

    class Member : IMember
    {
        private readonly Func<object, object> _getter;

        public Member(XName name, MemberInfo info, IType owner, Sort sort, Func<object, object> getter)
        {
            _getter = getter;
            Name = name;
            Info = info;
            Owner = owner;
            Sort = sort;
        }

        public XName Name { get; }
        public MemberInfo Info { get; }
        public IType Owner { get; }
        public Sort Sort { get; }

        public object Get(object instance) => _getter(instance);
    }

    public interface IAssignableMember : IMember
    {
        void Set(object instance, object value);
    }

    class AssignableMember : Member, IAssignableMember
    {
        private readonly Action<object, object> _setter;

        public AssignableMember(XName name, MemberInfo info, IType owner, Sort sort, Func<object, object> getter,
                                Action<object, object> setter) : base(name, info, owner, sort, getter)
        {
            _setter = setter;
        }

        public void Set(object instance, object value) => _setter(instance, value);
    }


    public interface IMembers : IParameterizedSource<string, IMember>, IEnumerable<IMember> {}

    public interface IMembersFactory : IParameterizedSource<IType, IMembers> {}

    class MembersFactory : IMembersFactory
    {
        public static MembersFactory Default { get; } = new MembersFactory();
        MembersFactory() : this(MemberNameProvider.Default, GetterFactory.Default, SetterFactory.Default) {}

        private readonly INameProvider _name;
        private readonly IGetterFactory _getter;
        private readonly ISetterFactory _setter;

        public MembersFactory(INameProvider name, IGetterFactory getter, ISetterFactory setter)
        {
            _name = name;
            _getter = getter;
            _setter = setter;
        }

        public IMembers Get(IType parameter) => new Members(Create(parameter).OrderBy(x => x.Sort));

        IEnumerable<IMember> Create(IType type)
        {
            foreach (var member in type.Subject.GetProperties())
            {
                var getMethod = member.GetGetMethod(true);
                if (member.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
                    !(!member.GetSetMethod(true)?.IsPublic ?? false) &&
                    member.GetIndexParameters().Length <= 0 &&
                    !member.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    yield return Create(type, member, !member.CanWrite);
                }
            }

            foreach (var member in type.Subject.GetFields())
            {
                var readOnly = member.IsInitOnly;
                if ((readOnly ? !member.IsLiteral : !member.IsStatic) &&
                    !member.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    yield return Create(type, member, readOnly);
                }
            }
        }

        private Member Create(IType type, MemberInfo member, bool readOnly)
        {
            var name = _name.Get(member);
            var sort = new Sort(member.GetCustomAttribute<XmlElementAttribute>(false)?.Order,
                                member.MetadataToken);
            var getter = _getter.Get(member);
            var result = readOnly
                ? new Member(name, member, type, sort, getter)
                : new AssignableMember(name, member, type, sort, getter, _setter.Get(member));
            return result;
        }
    }

    class Members : IMembers
    {
        private readonly IEnumerable<IMember> _source;
        private readonly Lazy<IMembers> _implementation;

        public Members(IEnumerable<IMember> source)
        {
            _source = source;
            _implementation = new Lazy<IMembers>(Create);
        }

        private IMembers Create() => new Implementation(_source);

        public IMember Get(string parameter) => _implementation.Value.Get(parameter);

        public IEnumerator<IMember> GetEnumerator() => _implementation.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        sealed class Implementation : IMembers
        {
            private readonly ImmutableArray<IMember> _items;
            private readonly IDictionary<string, IMember> _lookup;

            public Implementation(IEnumerable<IMember> items) : this(items.ToImmutableArray()) {}
            public Implementation(ImmutableArray<IMember> items) : this(items, items.ToDictionary(x => x.Info.Name)) {}

            public Implementation(ImmutableArray<IMember> items, IDictionary<string, IMember> lookup)
            {
                _items = items;
                _lookup = lookup;
            }

            public IMember Get(string parameter)
            {
                IMember result;
                return _lookup.TryGetValue(parameter, out result) ? result : null;
            }

            public IEnumerator<IMember> GetEnumerator() => ((IEnumerable<IMember>) _items).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
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

    public struct Sort : IComparable<Sort>
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