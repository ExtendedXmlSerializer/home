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
    public class ExtendedXmlSerializerOutputFormatter : TextOutputFormatter
    {
        private readonly IExtendedXmlSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.ExtendedXmlSerializerOutputFormatter" />
        /// with default XmlWriterSettings.
        /// </summary>
        public ExtendedXmlSerializerOutputFormatter()
          : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.ExtendedXmlSerializerOutputFormatter" />
        /// </summary>
        /// <param name="serializer">The serializer <see cref="IExtendedXmlSerializer"/></param>
        public ExtendedXmlSerializerOutputFormatter(IExtendedXmlSerializer serializer)
        {

            _serializer = serializer ?? new ExtendedXmlSerializer();
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationXml);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextXml);
        }

        /// <inheritdoc />
        protected override bool CanWriteType(Type type)
        {
            if (type == null)
                return false;
            return true;
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (selectedEncoding == null)
                throw new ArgumentNullException(nameof(selectedEncoding));

            object obj = context.Object;

            TextWriter textWriter = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding);
            try
            {
                textWriter.Write(_serializer.Serialize(obj));
                await textWriter.FlushAsync();
            }
            finally
            {
                textWriter?.Dispose();
            }
        }
    }
}