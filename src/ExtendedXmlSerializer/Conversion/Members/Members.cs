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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public class Members : WeakCacheBase<TypeInfo, IMemberElements>, IMembers
    {
        public static Members Default { get; } = new Members();
        Members() : this(Elements.Default) {}

        private readonly IElementSelector _selector;

        public Members(IElementSelector selector)
        {
            _selector = selector;
        }

        protected override IMemberElements Create(TypeInfo parameter) =>
            new MemberElements(Yield(parameter).OrderBy(x => x.Sort).Select(x => x.Member).ToImmutableArray());

        IEnumerable<Sorting> Yield(TypeInfo parameter)
        {
            foreach (var property in parameter.GetProperties())
            {
                var getMethod = property.GetGetMethod(true);
                if (property.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
                    !(!property.GetSetMethod(true)?.IsPublic ?? false) &&
                    property.GetIndexParameters().Length <= 0 &&
                    !property.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    yield return Create(property);
                }
            }

            foreach (var field in parameter.GetFields())
            {
                var readOnly = field.IsInitOnly;
                if ((readOnly ? !field.IsLiteral : !field.IsStatic) &&
                    !field.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    yield return Create(field);
                }
            }
        }

        private Sorting Create(MemberInfo metadata)
        {
            var sort = new Sort(metadata.GetCustomAttribute<XmlElementAttribute>(false)?.Order,
                                metadata.MetadataToken);

            var result = new Sorting((IMemberElement) _selector.Get(metadata), sort);
            return result;
        }

        struct Sorting
        {
            public Sorting(IMemberElement member, Sort sort)
            {
                Member = member;
                Sort = sort;
            }

            public IMemberElement Member { get; }
            public Sort Sort { get; }
        }
    }
}