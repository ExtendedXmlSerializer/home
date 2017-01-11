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
using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    class LegacyRootConverter : DecoratedConverter
    {
        public static LegacyRootConverter Default { get; } = new LegacyRootConverter();
        LegacyRootConverter() : this(Defaults.Tools) {}

        private readonly ISerializationToolsFactory _tools;

        public LegacyRootConverter(ISerializationToolsFactory tools)
            : this(tools, new RootConverter(LegacyElements.Default, new LegacySelectorFactory(tools))) {}

        public LegacyRootConverter(ISerializationToolsFactory tools, IConverter converter)
            : base(converter)
        {
            _tools = tools;
        }

        public override void Write(IWriteContext context, object instance)
        {
            var configuration = _tools.GetConfiguration(instance.GetType());
            if (configuration != null)
            {
                if (configuration.Version > 0)
                {
                    context.Write(VersionProperty.Default,
                                  configuration.Version.ToString(CultureInfo.InvariantCulture));
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
            var type = context.OwnerType;
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
            }
            return base.Read(context);
        }
    }
}