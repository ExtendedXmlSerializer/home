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
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.Members
{
    public class MemberInformationProvider : WeakCacheBase<TypeInfo, IMemberInformationView>, IMemberInformationProvider
    {
        public static MemberInformationProvider Default { get; } = new MemberInformationProvider();
        MemberInformationProvider() : this(MemberElementProvider.Default) {}

        private readonly IElementProvider _provider;

        public MemberInformationProvider(IElementProvider provider)
        {
            _provider = provider;
        }

        protected override IMemberInformationView Create(TypeInfo parameter) =>
            new MemberInformationView(Yield(parameter).OrderBy(x => x.Sort).Select(x => x.Member).ToImmutableArray());

        IEnumerable<SortedInformation> Yield(TypeInfo parameter)
        {
            foreach (var property in parameter.GetProperties())
            {
                var getMethod = property.GetGetMethod(true);
                if (property.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
                    !(!property.GetSetMethod(true)?.IsPublic ?? false) &&
                    property.GetIndexParameters().Length <= 0 &&
                    !property.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    yield return
                        Create(property, property.PropertyType.AccountForNullable(), property.CanWrite);
                }
            }

            foreach (var field in parameter.GetFields())
            {
                var readOnly = field.IsInitOnly;
                if ((readOnly ? !field.IsLiteral : !field.IsStatic) &&
                    !field.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    yield return Create(field, field.FieldType.AccountForNullable(), !readOnly);
                }
            }
        }

        private SortedInformation Create(MemberInfo metadata, Typing memberType, bool assignable)
        {
            var sort = new Sort(metadata.GetCustomAttribute<XmlElementAttribute>(false)?.Order,
                                metadata.MetadataToken);

            var result =
                new SortedInformation(new MemberInformation(_provider.Get(metadata), metadata, memberType, assignable),
                                      sort);
            return result;
        }

        struct SortedInformation
        {
            public SortedInformation(MemberInformation member, Sort sort)
            {
                Member = member;
                Sort = sort;
            }

            public MemberInformation Member { get; }
            public Sort Sort { get; }
        }
    }
}