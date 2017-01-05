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
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Model.Write;
using ExtendedXmlSerialization.Processing;

namespace ExtendedXmlSerialization.Model
{
    /*public interface ITypes : IParameterizedSource<TypeInfo, IType> {}

    public class Types : WeakCacheBase<TypeInfo, IType>, ITypes
    {
        public static Types Default { get; } = new Types();
        Types() : this(/*DictionaryTypeFactory.Default, EnumerableTypeFactory.Default,#1# ActivatedTypeFactory.Default) {}

        private readonly ImmutableArray<ITypeFactory> _factories;

        public Types(params ITypeFactory[] factories)
        {
            _factories = factories.ToImmutableArray();
        }

        protected override IType Create(TypeInfo parameter)
        {
            foreach (var factory in _factories)
            {
                if (factory.IsSatisfiedBy(parameter))
                {
                    return factory.Create(this, parameter);
                }
            }
            throw new InvalidOperationException($"Could not find a type factory for type '{parameter}'");
        }
    }

    public interface ITypeFactory : ISpecification<Typed>
    {
        IType Create(ITypes source, Typed type);
    }

    /*class BaseTypeFactory : TypeFactoryBase
    {
        public BaseTypeFactory(INameProvider names, IMembersFactory members) : base(names, members) {}

        public override bool IsSatisfiedBy(Typed parameter) => true;

        protected override IType Create(ITypes source, XName name, Typed typed, Func<IType, IMembers> members) =>
            new BaseType(name, typed.Info, members);
    }#1#

    class DictionaryTypeFactory : ActivatedTypeFactoryBase
    {
        public static DictionaryTypeFactory Default { get; } = new DictionaryTypeFactory();

        DictionaryTypeFactory()
            : this(
                EnumerableNameProvider.Default, InstanceMembers.Default, ActivatorFactory.Default,
                DictionaryPairTypesLocator.Default) {}

        private readonly IDictionaryPairTypesLocator _locator;

        public DictionaryTypeFactory(INameProvider names, IInstanceMembers members, IActivatorFactory factory,
                                     IDictionaryPairTypesLocator locator)
            : base(names, members, factory)
        {
            _locator = locator;
        }

        public override bool IsSatisfiedBy(Typed parameter) => _locator.Get(parameter.Type) != null;

        protected override IType Create(ITypes source, XName name, Typed type, Func<IType, IMembers> members,
                                        Func<object> activator)
        {
            var types = _locator.Get(type.Type);
            var keyType = source.Get(types.KeyType);
            var valueType = source.Get(types.ValueType);
            var result = new DictionaryType(name, type.Info, members, activator, keyType, valueType);
            return result;
        }
    }

    class EnumerableTypeFactory : ActivatedTypeFactoryBase
    {
        public static EnumerableTypeFactory Default { get; } = new EnumerableTypeFactory();
        EnumerableTypeFactory()
            : this(
                EnumerableNameProvider.Default, InstanceMembers.Default, ActivatorFactory.Default,
                ElementTypeLocator.Default, AddDelegateFactory.Default) {}

        private readonly IElementTypeLocator _locator;
        private readonly IAddDelegateFactory _add;

        public EnumerableTypeFactory(INameProvider names, IInstanceMembers members, IActivatorFactory factory,
                                     IElementTypeLocator locator, IAddDelegateFactory add)
            : base(names, members, factory)
        {
            _locator = locator;
            _add = add;
        }

        public override bool IsSatisfiedBy(Typed parameter) => _locator.Locate(parameter.Type) != null;

        protected override IType Create(ITypes source, XName name, Typed type, Func<IType, IMembers> members,
                                        Func<object> activator)
        {
            var elementType = _locator.Locate(type.Type);
            var parameter = new AddDelegateParameter(type.Type, elementType);
            var add = _add.Get(parameter);
            var result = new EnumerableType(name, type.Info, members, activator, source.Get(elementType.GetTypeInfo()),
                                            add);
            return result;
        }
    }*/

    public struct AddDelegateParameter
    {
        public AddDelegateParameter(Type type, Type elementType)
        {
            Type = type;
            ElementType = elementType;
        }

        public Type Type { get; }
        public Type ElementType { get; }
    }

    public interface IAddDelegateFactory : IParameterizedSource<AddDelegateParameter, Action<object, object>> {}

    class AddDelegateFactory : IAddDelegateFactory
    {
        public static AddDelegateFactory Default { get; } = new AddDelegateFactory();
        AddDelegateFactory() : this(AddMethodLocator.Default) {}

        private readonly IAddMethodLocator _locator;

        public AddDelegateFactory(IAddMethodLocator locator)
        {
            _locator = locator;
        }

        public Action<object, object> Get(AddDelegateParameter parameter)
        {
            var add = _locator.Locate(parameter.Type, parameter.ElementType);
            // Object (type object) from witch the data are retrieved
            var itemObject = Expression.Parameter(typeof(object), "item");

            // Object casted to specific type using the operator "as".
            var itemCasted = Expression.Convert(itemObject, parameter.Type);

            var value = Expression.Parameter(typeof(object), "value");

            var castedParam = Expression.Convert(value, parameter.ElementType);

            var conversion = Expression.Call(itemCasted, add, castedParam);

            var lambda = Expression.Lambda<Action<object, object>>(conversion, itemObject, value);

            var result = lambda.Compile();
            return result;
        }
    }


    /*class ActivatedTypeFactory : ActivatedTypeFactoryBase
    {
        public static ActivatedTypeFactory Default { get; } = new ActivatedTypeFactory();
        ActivatedTypeFactory() : this(NameProvider.Default, InstanceMembers.Default, ActivatorFactory.Default) {}

        public ActivatedTypeFactory(INameProvider names, IInstanceMembers members, IActivatorFactory factory)
            : base(names, members, factory) {}

        protected override IType Create(ITypes source, XName name, Typed type, Func<IType, IMembers> members,
                                        Func<object> activator)
            => new ActivatedType<object>(name, type.Info, members, activator);
    }

    abstract class ActivatedTypeFactoryBase : TypeFactoryBase
    {
        private readonly IActivatorFactory _factory;

        protected ActivatedTypeFactoryBase(INameProvider names, IInstanceMembers members, IActivatorFactory factory)
            : base(names, members)
        {
            _factory = factory;
        }

        public override bool IsSatisfiedBy(Typed parameter) =>
            !parameter.Info.IsAbstract &&
            (parameter.Info.IsValueType ||
             parameter.Info.IsClass && parameter.Info.GetConstructor(Type.EmptyTypes) != null);

        protected sealed override IType Create(ITypes source, XName name, Typed type, Func<IType, IMembers> members) =>
            Create(source, name, type, members, _factory.Get(type.Type));

        protected abstract IType Create(ITypes source, XName name, Typed type, Func<IType, IMembers> members,
                                        Func<object> activator);
    }

    abstract class TypeFactoryBase : ITypeFactory
    {
        readonly private INameProvider _names;
        readonly private Func<IType, IMembers> _members;
        protected TypeFactoryBase(INameProvider names, IInstanceMembers members) : this(names, members.Get) {}

        protected TypeFactoryBase(INameProvider names, Func<IType, IMembers> members)
        {
            _names = names;
            _members = members;
        }

        public abstract bool IsSatisfiedBy(Typed parameter);

        public IType Create(ITypes source, Typed type) => Create(source, _names.Get(type.Info), type, _members);
        protected abstract IType Create(ITypes source, XName name, Typed type, Func<IType, IMembers> members);
    }*/

    public interface IType
    {
        XName Name { get; }

        TypeInfo Subject { get; }

        IMembers Members { get; }
    }

    class BaseType : IType
    {
        private readonly Func<IType, IMembers> _members;
        private readonly Lazy<IMembers> _source;

        public BaseType(XName name, TypeInfo subject, Func<IType, IMembers> members)
        {
            _members = members;
            Name = name;
            Subject = subject;
            _source = new Lazy<IMembers>(Create);
        }

        private IMembers Create() => _members(this);

        public XName Name { get; }
        public TypeInfo Subject { get; }
        public IMembers Members => _source.Value;
    }

    public interface IDictionaryType : IActivatedType<IDictionary>
    {
        IType KeyType { get; }

        IType ValueType { get; }
    }

    class DictionaryType : ActivatedType<IDictionary>, IDictionaryType
    {
        public DictionaryType(XName name, TypeInfo subject, Func<IType, IMembers> members, Func<object> activator,
                              IType keyType, IType valueType)
            : base(name, subject, members, activator)
        {
            KeyType = keyType;
            ValueType = valueType;
        }

        public IType KeyType { get; }
        public IType ValueType { get; }
    }

    public interface IEnumerableType : IActivatedType<IEnumerable>
    {
        IType ElementType { get; }

        void Add(object instance, object item);
    }

    class EnumerableType : ActivatedType<IEnumerable>, IEnumerableType
    {
        private readonly Action<object, object> _add;

        public EnumerableType(XName name, TypeInfo subject, Func<IType, IMembers> members, Func<object> activator,
                              IType elementType, Action<object, object> add)
            : base(name, subject, members, activator)
        {
            _add = add;
            ElementType = elementType;
        }

        public IType ElementType { get; }
        public void Add(object instance, object item) => _add(instance, item);
    }


    public interface IActivatedType : IType
    {
        object New();
    }

    public interface IActivatedType<out T> : IActivatedType
    {
        new T New();
    }

    public interface IActivatorFactory : IParameterizedSource<Type, Func<object>> {}

    class ActivatorFactory : IActivatorFactory
    {
        public static ActivatorFactory Default { get; } = new ActivatorFactory();
        ActivatorFactory() {}

        public Func<object> Get(Type parameter)
        {
            var newExp = Expression.Convert(Expression.New(parameter), typeof(object));
            var lambda = Expression.Lambda<Func<object>>(newExp);
            var result = lambda.Compile();
            return result;
        }
    }

    class ActivatedType<T> : BaseType, IActivatedType<T>
    {
        private readonly Func<object> _activator;

        public ActivatedType(XName name, TypeInfo subject, Func<IType, IMembers> members, Func<object> activator)
            : base(name, subject, members)
        {
            _activator = activator;
        }

        public T New() => (T) _activator();

        object IActivatedType.New() => New();
    }
}