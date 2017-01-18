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

using System.Xml.Linq;
using ExtendedXmlSerialization.Conversion;
using ExtendedXmlSerialization.Conversion.Read;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.ElementModel;

namespace ExtendedXmlSerialization.Legacy
{
    sealed class LegacyRootConverter : RootConverter
    {
        private readonly ISerializationToolsFactory _tools;

        public LegacyRootConverter(ISerializationToolsFactory tools)
            : this(tools, LegacyElementsTooling.Default.Get(tools)) {}

        LegacyRootConverter(ISerializationToolsFactory tools, IElements elements)
            : this(tools, new RootSelector(new SelectorFactory(elements, new LegacyConverterOptions(tools)))) {}

        LegacyRootConverter(ISerializationToolsFactory tools, ISelector selector) : base(selector)
        {
            _tools = tools;
        }

        public override void Write(IWriteContext context, object instance)
        {
            var type = instance.GetType();
            var configuration = _tools.GetConfiguration(type);
            if (configuration != null)
            {
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
            var type = context.Element.Classification.AsType();
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