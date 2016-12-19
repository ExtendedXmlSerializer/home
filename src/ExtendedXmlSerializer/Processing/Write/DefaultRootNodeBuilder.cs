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

        readonly private IDictionary<long, IInstance> _instances = new Dictionary<long, IInstance>();
        readonly private ObjectIdGenerator _generator = new ObjectIdGenerator();

        public IObjectNode Get(object parameter)
        {
            var type = parameter.GetType();
            var definition = Definition(type);
            var result = Select(parameter, definition);
            return result;
        }

        IObjectNode Select(object instance, IMemberDefinition member) => Select(instance, member, member.TypeDefinition);
        private IObjectNode Select(object instance, ITypeDefinition type) => Select(instance, type, type);

        IObjectNode Select(object instance, IDefinition source, ITypeDefinition type)
        {
            if (type.IsPrimitive)
            {
                return new Primitive(instance, source.Type, source.Name);
            }

            bool first;
            var id = _generator.GetId(instance, out first);
            if (first)
            {
                var @select = _instances[id] = GetInstance(id, instance, source, type);
                return @select;
            }

            if (_instances.ContainsKey(id))
            {
                return new Reference(_instances[id]);
            }
            throw new InvalidOperationException(
                      $"Could not create a context for instance '{instance}' of type '{type.Type}'.");
        }

        private IInstance GetInstance(long id, object instance, IDefinition source, ITypeDefinition type)
        {
            var ensured = type.For(instance);
            var members = CreateMembers(instance, ensured) /*.OrderBy(x => x is IProperty)*/;

            var dictionary = instance as IDictionary;
            if (dictionary != null)
            {
                var key = Definition(type.GenericArguments[0]);
                var value = Definition(type.GenericArguments[1]);
                var entries = CreateEntries(dictionary, key, value);
                return new DictionaryInstance(id, dictionary, source.Type, source.Name, members.Concat(entries));
            }

            if (IsEnumerable(ensured))
            {
                var elementType = Definition(type.GenericArguments[0]);
                var elements = CreateElements(instance, elementType);
                return new EnumerableInstance(id, elementType.Type, (IEnumerable) instance, source.Type,
                                              source.Name,
                                              members.Concat(elements));
            }
            var result = new Instance(id, instance, source.Type, source.Name, members);
            return result;
        }

        IEnumerable<IObjectNode> CreateMembers(object instance, ITypeDefinition definition)
        {
            foreach (var member in definition.Members)
            {
                yield return Select(member.GetValue(instance), member);
            }
        }

        private IEnumerable<IObjectNode> CreateElements(object instance, ITypeDefinition elementType)
        {
            foreach (var element in Arrays.Default.AsArray(instance))
            {
                yield return Select(element, elementType);
            }
        }

        private IEnumerable<IDictionaryEntry> CreateEntries(IDictionary dictionary, ITypeDefinition keyDefinition,
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