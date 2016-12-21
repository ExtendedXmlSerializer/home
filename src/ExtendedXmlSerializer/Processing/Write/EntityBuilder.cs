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
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    public class EntityBuilder : IEntityBuilder
    {
        private readonly IEntitySelector _selector;
        private readonly ISpecification<InstanceDescriptor> _member;

        readonly private static Func<ITypeDefinition, bool> IsEnumerable =
            IsEnumerableTypeSpecification.Default.IsSatisfiedBy;

        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;

        public EntityBuilder(IEntitySelector selector) : this(selector, DefaultMemberSpecification.Default) {}

        public EntityBuilder(IEntitySelector selector, ISpecification<InstanceDescriptor> member)
        {
            _selector = selector;
            _member = member;
        }

        public IEntity Get(InstanceDescriptor parameter)
        {
            var members = CreateMembers(parameter.Instance, parameter.ActualType);

            var dictionary = parameter.Instance as IDictionary;
            if (dictionary != null)
            {
                var arguments = parameter.DeclaredType.GenericArguments;
                var key = Definition(arguments[0]);
                var value = Definition(arguments[1]);
                var entries = CreateEntries(dictionary, key, value);
                return new DictionaryObject(dictionary, parameter.DeclaredType, parameter.ActualType, parameter.Name,
                                            members.Concat(entries));
            }

            if (IsEnumerable(parameter.ActualType))
            {
                var elementType = Definition(parameter.DeclaredType.GenericArguments[0]);
                var elements = CreateElements(parameter.Instance, elementType);
                return new EnumerableObject((IEnumerable) parameter.Instance, parameter.DeclaredType,
                                            parameter.ActualType, parameter.Name,
                                            members.Concat(elements));
            }
            var result = new Object<object>(parameter.Instance, parameter.DeclaredType, parameter.ActualType,
                                            parameter.Name, members);
            return result;
        }

        IEnumerable<IEntity> CreateMembers(object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                var descriptor = new InstanceDescriptor(member.GetValue(instance), member.TypeDefinition, member.Name);
                if (_member.IsSatisfiedBy(descriptor))
                {
                    yield return new Member(_selector.Get(descriptor), member);
                }
            }
        }

        private IEnumerable<IEntity> CreateElements(object instance, ITypeDefinition elementType)
        {
            foreach (var element in Arrays.Default.AsArray(instance))
            {
                yield return _selector.Get(new InstanceDescriptor(element, elementType));
            }
        }

        private IEnumerable<IEntity> CreateEntries(IDictionary dictionary, ITypeDefinition keyDefinition,
                                                   ITypeDefinition valueDefinition)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = new DictionaryKey(_selector.Get(new InstanceDescriptor(entry.Key, keyDefinition)));
                var value = new DictionaryValue(_selector.Get(new InstanceDescriptor(entry.Value, valueDefinition)));
                var result = new DictionaryEntryInstance(entry, key, value);
                yield return result;
            }
        }
    }
}