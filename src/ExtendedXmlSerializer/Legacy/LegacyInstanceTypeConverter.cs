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

using System.Globalization;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ElementModel;
using ExtendedXmlSerialization.ElementModel.Members;

namespace ExtendedXmlSerialization.Legacy
{
    sealed class LegacyInstanceTypeConverter : Converter
    {
        public LegacyInstanceTypeConverter(IConverter converter)
            : base(new InstanceBodyReader(converter), new TypeEmittingWriter(new InstanceBodyWriter(converter))) {}

        public LegacyInstanceTypeConverter(IInternalExtendedXmlSerializerConfig config, IConverter converter)
            : base(
                new LegacyInstanceBodyReader(config, converter),
                new LegacyTypeEmittingWriter(new Writer(config, new InstanceBodyWriter(converter)))
            ) {}

        private sealed class Writer : DecoratedWriter
        {
            private readonly IInternalExtendedXmlSerializerConfig _tools;

            public Writer(IInternalExtendedXmlSerializerConfig tools, IWriter writer) : base(writer)
            {
                _tools = tools;
            }

            public override void Write(IWriteContext context, object instance)
            {
                var type = instance.GetType();
                var configuration = _tools.GetTypeConfig(type);
                if (configuration != null)
                {
                    if (configuration.IsObjectReference)
                    {
                        var references = context.Get<WriteReferences>();

                        var objectId = configuration.GetObjectId(instance);

                        if (!(context.Container is IMemberElement) && references.Reserved.Contains(instance))
                        {
                            references.Reserved.Remove(instance);
                        }
                        else if (references.Contains(instance) || references.Reserved.Contains(instance))
                        {
                            context.Write(ReferenceProperty.Default, objectId);
                            return;
                        }

                        context.Write(IdentifierProperty.Default, objectId);
                        references.Add(instance);
                    }

                    if (configuration.Version > 0)
                    {
                        context.Write(VersionProperty.Default,
                                      configuration.Version.ToString(CultureInfo.InvariantCulture));
                    }
                }

                base.Write(context, instance);
            }
        }
    }
}