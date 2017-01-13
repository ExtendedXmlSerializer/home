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
using ExtendedXmlSerialization.Conversion.Legacy;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    public interface IElementNameOption : IOption<MemberInfo, IElementName> {}

    public abstract class ElementNameOptionBase : Option<MemberInfo, IElementName>, IElementNameOption
    {
        protected ElementNameOptionBase(Func<MemberInfo, IElementName> source)
            : this(AlwaysSpecification<MemberInfo>.Default, source) {}

        protected ElementNameOptionBase(ISpecification<MemberInfo> specification, Func<MemberInfo, IElementName> source)
            : base(specification, source) {}
    }

    public class ElementNameOption<T> : ElementNameOptionBase
    {
        public static ElementNameOption<T> Default { get; } = new ElementNameOption<T>();

        ElementNameOption()
            : base(
                IsAssignableSpecification<Enum>.Default.Adapt(), x => new ElementName(x.ToTypeInfo())) {}
    }

    public class RegisteredElementNames : ElementNameOptionBase
    {
        public static RegisteredElementNames Default { get; } = new RegisteredElementNames();
        RegisteredElementNames() : this(Defaults.Names) {}

        public RegisteredElementNames(IEnumerable<IElementName> elements)
            : this(elements.ToDictionary(x => (MemberInfo) x.KeyedType)) {}

        public RegisteredElementNames(IDictionary<MemberInfo, IElementName> names)
            : base(new DelegatedSpecification<MemberInfo>(names.ContainsKey), names.TryGet) {}
    }

    public class DefaultElementNameOption : ElementNameOptionBase
    {
        public static DefaultElementNameOption Default { get; } = new DefaultElementNameOption();
        DefaultElementNameOption() : base(ElementNameProvider.Default.Get) {}
    }

    public class EnumerableNameOption : ElementNameOptionBase
    {
        public static EnumerableNameOption Default { get; } = new EnumerableNameOption();
        EnumerableNameOption() : this(LegacyEnumerableElementNameProvider.Default.Get) {}

        public EnumerableNameOption(Func<MemberInfo, IElementName> source)
            : base(Specification.Instance, source) {}

        sealed class Specification : ISpecification<MemberInfo>
        {
            readonly private static TypeInfo TypeInfo = typeof(IEnumerable).GetTypeInfo();

            public static Specification Instance { get; } = new Specification();
            Specification() {}

            public bool IsSatisfiedBy(MemberInfo parameter)
            {
                var type = parameter.ToTypeInfo();
                var result = type.IsArray || type.IsGenericType && TypeInfo.IsAssignableFrom(type);
                return result;
            }
        }
    }

    public interface IElementOption : IOption<MemberInfo, IElement> {}

    public abstract class ElementOptionBase<T> : OptionBase<T, IElement>, IElementOption where T : MemberInfo
    {
        private readonly Func<MemberInfo, IElementName> _name;

        protected ElementOptionBase(params IElementNameOption[] names)
            : this(AlwaysSpecification<MemberInfo>.Default, names) {}

        protected ElementOptionBase(ISpecification<T> specification, params IElementNameOption[] names)
            : this(specification, new Selector<MemberInfo, IElementName>(names).Get) {}

        protected ElementOptionBase(ISpecification<T> specification, Func<MemberInfo, IElementName> name)
            : base(specification)
        {
            _name = name;
        }

        public override IElement Get(T parameter) => Create(parameter, _name(parameter));

        protected abstract IElement Create(T parameter, IElementName name);

        bool ISpecification<MemberInfo>.IsSatisfiedBy(MemberInfo parameter) => IsSatisfiedBy(parameter.AsValid<T>());

        IElement IParameterizedSource<MemberInfo, IElement>.Get(MemberInfo parameter) => Get(parameter.AsValid<T>());
    }
}