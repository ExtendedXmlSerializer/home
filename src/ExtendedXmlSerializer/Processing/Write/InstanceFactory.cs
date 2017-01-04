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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Model.Write;
using IInstance = ExtendedXmlSerialization.Model.Write.IInstance;
using IItem = ExtendedXmlSerialization.Model.Write.IItem;
using IMember = ExtendedXmlSerialization.Model.Write.IMember;
using Member = ExtendedXmlSerialization.Model.Write.Member;
using Object = ExtendedXmlSerialization.Model.Write.Object;

namespace ExtendedXmlSerialization.Processing.Write
{
    public class InstanceFactory : IInstanceFactory
    {
        public static InstanceFactory Default { get; } = new InstanceFactory();
        InstanceFactory() : this(DefaultMemberSpecification.Default) {}

        private readonly ISpecification<Descriptor> _member;


        readonly private static Func<ITypeDefinition, bool> IsEnumerable =
            IsEnumerableTypeSpecification.Default.IsSatisfiedBy;

        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;


        public InstanceFactory(ISpecification<Descriptor> member)
        {
            _member = member;
        }

        public IInstance Create(IPrimaryInstanceFactory factory, Descriptor parameter)
        {
            var members = CreateMembers(factory, parameter.Instance, parameter.ActualType);

            var dictionary = parameter.Instance as IDictionary;
            if (dictionary != null)
            {
                var arguments = parameter.DeclaredType.GenericArguments;
                var key = Definition(arguments[0]);
                var value = Definition(arguments[1]);
                var entries = CreateEntries(factory, dictionary, key, value);
                return new DictionaryObject(dictionary, parameter.ActualType.Type, members, entries);
            }

            if (IsEnumerable(parameter.ActualType))
            {
                var definition = Definition(parameter.DeclaredType.GenericArguments[0]);
                var items = CreateItems(factory, parameter.Instance, definition);
                return new EnumerableObject((IEnumerable) parameter.Instance,
                                            parameter.ActualType.Type,
                                            members, items);
            }
            var result = new Object(parameter.Instance, parameter.ActualType.Type, members);
            return result;
        }

        IEnumerable<IMember> CreateMembers(IPrimaryInstanceFactory factory, object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                var descriptor = new Descriptor(member.GetValue(instance), member.TypeDefinition, member.Name);
                if (_member.IsSatisfiedBy(descriptor))
                {
                    yield return
                        new Member(factory.Get(descriptor), member.Name, descriptor.DeclaredType.Type,
                                   member.Metadata.DeclaringType, member.IsWritable);
                }
            }
        }

        private static IEnumerable<IItem> CreateItems(IPrimaryInstanceFactory factory, object instance,
                                                      ITypeDefinition definition)
        {
            foreach (var item in Arrays.Default.AsArray(instance))
            {
                var descriptor = new Descriptor(item, definition, definition.For(item));
                yield return new Item(factory.Get(descriptor), definition.Type, descriptor.Name);
            }
        }

        private static IEnumerable<DictionaryEntryItem> CreateEntries(
            IPrimaryInstanceFactory factory,
            IDictionary dictionary, ITypeDefinition keyDefinition,
            ITypeDefinition valueDefinition)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = new DictionaryKey(factory.Get(new Descriptor(entry.Key, keyDefinition)),
                                            keyDefinition.Type);
                var value = new DictionaryValue(factory.Get(new Descriptor(entry.Value, valueDefinition)),
                                                valueDefinition.Type);
                var result = new DictionaryEntryItem(key, value);
                yield return result;
            }
        }
    }
}