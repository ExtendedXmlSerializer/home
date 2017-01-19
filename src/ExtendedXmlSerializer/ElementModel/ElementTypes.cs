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

using System.Reflection;
using System.Xml.Linq;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ElementModel
{
    public class ElementTypes : IElementTypes
    {
        public static ElementTypes Default { get; } = new ElementTypes();
        ElementTypes() : this(NamedTypeLocator.Default, TypeParser.Default) {}

        private readonly INamedTypeLocator _types;
        private readonly ITypeParser _parser;

        public ElementTypes(INamedTypeLocator types, ITypeParser parser)
        {
            _parser = parser;
            _types = types;
        }

        public TypeInfo Get(XElement parameter)
        {
            var stored = parameter.Annotation<TypeInfo>();
            if (stored == null)
            {
                var result = _types.Get(parameter.Name) ?? FromAttribute(parameter);
                if (result != null)
                {
                    parameter.AddAnnotation(result);
                    return result;
                }
            }
            return stored;
        }

        private TypeInfo FromAttribute(XElement parameter)
        {
            var value = parameter.Attribute(TypeProperty.Default.DisplayName)?.Value;
            var result = value != null ? _parser.Get(value) : null;
            return result;
        }
    }
}