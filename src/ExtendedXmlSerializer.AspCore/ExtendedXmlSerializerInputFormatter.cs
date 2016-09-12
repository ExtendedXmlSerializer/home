// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace ExtendedXmlSerialization.AspCore
{
    /// <summary>
    /// This class handles deserialization of input XML data
    /// to strongly-typed objects using <see cref="T:System.Xml.Serialization.XmlSerializer" />
    /// </summary>
    public class ExtendedXmlSerializerInputFormatter : TextInputFormatter
    {
        private readonly IExtendedXmlSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of ExtendedXmlSerializerInputFormatter.
        /// </summary>
        public ExtendedXmlSerializerInputFormatter() :this(null)
        {

        }

        public ExtendedXmlSerializerInputFormatter(IExtendedXmlSerializer serializer)
        {
            _serializer = serializer ?? new ExtendedXmlSerializer();

            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationXml);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextXml);
        }

        /// <inheritdoc />
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            StreamReader reader = new StreamReader(context.HttpContext.Request.Body, encoding);
            string text = reader.ReadToEnd();
            
            object model = _serializer.Deserialize(text, context.ModelType);

            return InputFormatterResult.SuccessAsync(model);
        }

        /// <inheritdoc />
        protected override bool CanReadType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return true;
        }
    }
}