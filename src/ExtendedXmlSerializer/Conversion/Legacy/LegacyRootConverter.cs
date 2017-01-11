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

using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.TypeModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    sealed class LegacyRootConverter : DecoratedConverter
    {
        const string Underscore = "_";

        private readonly ISerializationToolsFactory _tools;

        public LegacyRootConverter(ISerializationToolsFactory tools)
            : this(tools, new RootConverter(LegacyElements.Default, new LegacySelectorFactory(tools))) {}

        public LegacyRootConverter(ISerializationToolsFactory tools, IConverter converter) : base(converter)
        {
            _tools = tools;
        }

        public override void Write(IWriteContext context, object instance)
        {
            var type = instance.GetType();
            var configuration = _tools.GetConfiguration(type);
            if (configuration != null)
            {
                if (configuration.IsObjectReference)
                {
                    var references = context.Get<References>();

                    var objectId = configuration.GetObjectId(instance);

                    if (IsEnumerableTypeSpecification.Default.IsSatisfiedBy(type.GetTypeInfo()))
                    {
                        foreach (var item in (IEnumerable) instance)
                        {
                            var itemKey = item.GetType().FullName + Underscore + configuration.GetObjectId(item);
                            if (!references.ContainsKey(itemKey) && !references.Reserved.ContainsKey(itemKey))
                            {
                                references.Items.Add(itemKey);
                                references.Add(itemKey, item);
                            }
                        }
                    }

                    var key = type.FullName + Underscore + objectId;
                    if (references.Items.Contains(key) && references.Reserved.ContainsKey(key))
                    {
                        references.Reserved.Remove(key);
                    }
                    else if (references.ContainsKey(key) || references.Reserved.ContainsKey(key))
                    {
                        new ReferenceWriter(objectId).Write(context, instance);
                        return;
                    }

                    // properties[IdentifierProperty.Default] = objectId;
                    references.Add(key, instance);
                }


                if (configuration.IsCustomSerializer)
                {
                    new CustomElementWriter(configuration).Write(context, instance);
                    return;
                }
            }

            base.Write(context, instance);
        }

        public override object Read(IReadContext context)
        {
            var type = context.ReferencedType;
            var configuration = _tools.GetConfiguration(type);
            if (configuration != null)
            {
                var element = context.Get<XElement>();
                // Run migrator if exists
                if (configuration.Version > 0)
                {
                    configuration.Map(type, element);
                }

                if (configuration.IsCustomSerializer)
                {
                    return configuration.ReadObject(element);
                }

                if (configuration.IsObjectReference)
                {
                    var prefix = context.ReferencedType.Type.FullName + Underscore;
                    var refId = context[ReferenceProperty.Default];
                    var references = context.Get<References>();
                    if (!string.IsNullOrEmpty(refId))
                    {
                        var key = prefix + refId;
                        if (references.ContainsKey(key))
                        {
                            return references[key];
                        }
                        var instance = base.Read(context);
                        references.Add(key, instance);
                        return instance;
                    }

                    var objectId = context[IdentifierProperty.Default];
                    if (!string.IsNullOrEmpty(objectId))
                    {
                        var key = prefix + Underscore + objectId;
                        if (references.ContainsKey(key))
                        {
                            return references[key];
                        }
                        var instance = base.Read(context);
                        references.Add(key, instance);
                        return instance;
                    }
                }
            }
            return base.Read(context);
        }
    }
}