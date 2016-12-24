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
using Object = ExtendedXmlSerialization.Model.Write.Object;

namespace ExtendedXmlSerialization.Processing.Write
{
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly IInstanceSelector _selector;
        private readonly ISpecification<Descriptor> _member;

        readonly private static Func<ITypeDefinition, bool> IsEnumerable =
            IsEnumerableTypeSpecification.Default.IsSatisfiedBy;

        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;

        public InstanceBuilder(IInstanceSelector selector) : this(selector, DefaultMemberSpecification.Default) {}

        public InstanceBuilder(IInstanceSelector selector, ISpecification<Descriptor> member)
        {
            _selector = selector;
            _member = member;
        }

        public IInstance Get(Descriptor parameter)
        {
            var members = CreateMembers(parameter.Instance, parameter.ActualType);

            var dictionary = parameter.Instance as IDictionary;
            if (dictionary != null)
            {
                var arguments = parameter.DeclaredType.GenericArguments;
                var key = Definition(arguments[0]);
                var value = Definition(arguments[1]);
                var entries = CreateEntries(dictionary, key, value);
                return new DictionaryObject(dictionary, parameter.ActualType.Type, members, entries);
            }

            if (IsEnumerable(parameter.ActualType))
            {
                var definition = Definition(parameter.DeclaredType.GenericArguments[0]);
                var items = CreateItems(parameter.Instance, definition);
                return new EnumerableObject((IEnumerable) parameter.Instance,
                                            parameter.ActualType.Type,
                                            members, items);
            }
            var result = new Object(parameter.Instance, parameter.ActualType.Type, members);
            return result;
        }

        IEnumerable<IMember> CreateMembers(object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                var descriptor = new Descriptor(member.GetValue(instance), member.TypeDefinition, member.Name);
                if (_member.IsSatisfiedBy(descriptor))
                {
                    yield return new Member(_selector.Get(descriptor), member.Name, descriptor.DeclaredType.Type, member.Metadata.DeclaringType, member.IsWritable);
                }
            }
        }

        private IEnumerable<IItem> CreateItems(object instance, ITypeDefinition definition)
        {
            foreach (var item in Arrays.Default.AsArray(instance))
            {
                var descriptor = new Descriptor(item, definition, definition.For(item));
                yield return new Item(_selector.Get(descriptor), definition.Type, descriptor.Name);
            }
        }

        private IEnumerable<DictionaryEntryItem> CreateEntries(IDictionary dictionary, ITypeDefinition keyDefinition,
                                                               ITypeDefinition valueDefinition)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = new DictionaryKey(_selector.Get(new Descriptor(entry.Key, keyDefinition)),
                                            keyDefinition.Type);
                var value = new DictionaryValue(_selector.Get(new Descriptor(entry.Value, valueDefinition)),
                                                valueDefinition.Type);
                var result = new DictionaryEntryItem(key, value);
                yield return result;
            }
        }
    }
}