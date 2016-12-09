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

using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using ExtendedXmlSerialization.Instructions;
using ExtendedXmlSerialization.Services;
using ExtendedXmlSerialization.Services.Services;
using WriteState = ExtendedXmlSerialization.Services.Services.WriteState;

namespace ExtendedXmlSerialization.Extensibility.Write
{
    public class CustomSerializationExtension : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;
        private readonly IInstruction _instruction;

        public CustomSerializationExtension(ISerializationToolsFactory factory, IInstruction instruction)
        {
            _factory = factory;
            _instruction = instruction;
        }

        public override bool Starting(IWriting services)
        {
            switch (services.Current.State)
            {
                case WriteState.Members:
                    var instance = services.Current.Instance;
                    var configuration = _factory.GetConfiguration(instance.GetType());
                    if (configuration?.IsCustomSerializer ?? false)
                    {
                        _instruction.Execute(services);
                        configuration.WriteObject(services.Get<XmlWriter>(), instance);
                        return false;
                    }
                   break;
            }
            return true;
        }
    }
}