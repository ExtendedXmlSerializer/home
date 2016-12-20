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
using ExtendedXmlSerialization.Model;
using ExtendedXmlSerialization.Model.Write;

namespace ExtendedXmlSerialization.Processing.Write
{
    class DefaultRootNodeBuilder : IRootNodeBuilder
    {
        readonly private static Func<ITypeDefinition, bool> IsEnumerable =
            IsEnumerableTypeSpecification.Default.IsSatisfiedBy;

        readonly private static Func<Type, ITypeDefinition> Definition = TypeDefinitions.Default.Get;

        // readonly private IDictionary<long, IReference> _instances = new Dictionary<long, IReference>();
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public IEntity Get(object parameter)
        {
            var type = parameter.GetType();
            var definition = Definition(type);
            var result = Select(parameter, definition, definition, definition.Name);
            return result;
        }

        private IEntity Select(object instance, ITypeDefinition declared) => Select(instance, declared, declared.Name);

        private IEntity Select(object instance, ITypeDefinition declared, string name)
            => Select(instance, declared, declared.For(instance), name);

        private IEntity Select(object instance, ITypeDefinition declared, ITypeDefinition actual, string name)
        {
            if (actual.IsPrimitive)
            {
                return new Primitive(instance, declared, actual, name);
            }

            bool first;
            var id = _generator.GetId(instance, out first);
            if (first)
            {
                var o = GetInstance(instance, declared, actual, name);
                // _instances[id] = new Reference(id, o);
                return o;
            }

            /*if (_instances.ContainsKey(id))
            {
                return new ReferenceLookup(_instances[id], name);
            }
            */

            throw new InvalidOperationException(
                      $"Recursion detected on '{instance}' of type '{declared.Type}'. This builder does not support recursion.  Please configure your serializer with a builder that does support recursion such as TODO.");
        }

        private IObject GetInstance(object instance, ITypeDefinition declaredType, ITypeDefinition actualType,
                                    string name)
        {
            var members = CreateMembers(instance, actualType);

            var dictionary = instance as IDictionary;
            if (dictionary != null)
            {
                var key = Definition(declaredType.GenericArguments[0]);
                var value = Definition(declaredType.GenericArguments[1]);
                var entries = CreateEntries(dictionary, key, value);
                return new DictionaryObject(dictionary, declaredType, actualType, name, members.Concat(entries));
            }

            if (IsEnumerable(actualType))
            {
                var elementType = Definition(declaredType.GenericArguments[0]);
                var elements = CreateElements(instance, elementType);
                return new EnumerableObject((IEnumerable) instance, declaredType, actualType, name,
                                            members.Concat(elements));
            }
            var result = new Object<object>(instance, declaredType, actualType, name, members);
            return result;
        }

        IEnumerable<IEntity> CreateMembers(object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                var value = member.GetValue(instance);
                if (value != null || definition.IsEnum)
                {
                    yield return new Member(Select(value, member.TypeDefinition, member.Name), member);
                }
            }
        }

        private IEnumerable<IEntity> CreateElements(object instance, ITypeDefinition elementType)
        {
            foreach (var element in Arrays.Default.AsArray(instance))
            {
                yield return Select(element, elementType);
            }
        }

        private IEnumerable<IEntity> CreateEntries(IDictionary dictionary, ITypeDefinition keyDefinition,
                                                   ITypeDefinition valueDefinition)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = new DictionaryKey(Select(entry.Key, keyDefinition));
                var value = new DictionaryValue(Select(entry.Value, valueDefinition));
                var result = new DictionaryEntryInstance(entry, key, value);
                yield return result;
            }
        }
    }
}