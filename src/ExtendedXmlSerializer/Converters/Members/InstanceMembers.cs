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
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Converters.Members
{
    class InstanceMembers : WeakCacheBase<TypeInfo, IMembers>, IInstanceMembers
    {
        private readonly IMemberFactory _factory;

        public InstanceMembers(IMemberFactory factory)
        {
            _factory = factory;
        }

        protected override IMembers Create(TypeInfo parameter) =>
            new Members(CreateMembers(new Typed(parameter)).OrderBy(x => x.Sort).Select(x => x.Member));

        IEnumerable<MemberSort> CreateMembers(Typed declaringType)
        {
            foreach (var property in declaringType.Info.GetProperties())
            {
                var getMethod = property.GetGetMethod(true);
                if (property.CanRead && !getMethod.IsStatic && getMethod.IsPublic &&
                    !(!property.GetSetMethod(true)?.IsPublic ?? false) &&
                    property.GetIndexParameters().Length <= 0 &&
                    !property.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    var type = new Typed(property.PropertyType.AccountForNullable());
                    var member = Create(property, type, property.CanWrite);
                    if (member != null)
                    {
                        yield return member.Value;
                    }
                }
            }

            foreach (var field in declaringType.Info.GetFields())
            {
                var readOnly = field.IsInitOnly;
                if ((readOnly ? !field.IsLiteral : !field.IsStatic) &&
                    !field.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    var type = new Typed(field.FieldType.AccountForNullable());
                    var member = Create(field, type, !readOnly);
                    if (member != null)
                    {
                        yield return member.Value;
                    }
                }
            }
        }

        private MemberSort? Create(MemberInfo metadata, Typed memberType, bool assignable)
        {
            var member = _factory.Create(metadata, memberType, assignable);
            if (member != null)
            {
                var sort = new Sort(metadata.GetCustomAttribute<XmlElementAttribute>(false)?.Order,
                                    metadata.MetadataToken);

                var result = new MemberSort(member, sort);
                return result;
            }

            // TODO: Warning? Throw?
            return null;
        }

        struct MemberSort
        {
            public MemberSort(IMember member, Sort sort)
            {
                Member = member;
                Sort = sort;
            }

            public IMember Member { get; }
            public Sort Sort { get; }
        }
    }
}