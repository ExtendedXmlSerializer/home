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
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace ExtendedXmlSerialization.WebApi
{
    public class ExtendedXmlSerializerXmlMediaTypeFormatter : XmlMediaTypeFormatter
    {
        private readonly IExtendedXmlSerializer _serializer;

        public ExtendedXmlSerializerXmlMediaTypeFormatter(IExtendedXmlSerializer serializer)
        {
            _serializer = serializer;
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return ReadFromStreamAsync(type, readStream, content, formatterLogger, new CancellationToken(false));
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (readStream == null)
            {
                throw new ArgumentNullException(nameof(readStream));
            }

            var completion = new TaskCompletionSource<object>();

            if (cancellationToken.IsCancellationRequested)
            {
                completion.SetCanceled();
            }
            else
            {
                try
                {
                    var value = ReadValue(type, readStream, content, formatterLogger);
                    completion.SetResult(value);
                }
                catch (Exception ex)
                {
                    completion.SetException(ex);
                }
            }

            return completion.Task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return WriteToStreamAsync(type, value, writeStream, content, transportContext, new CancellationToken(false));
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (writeStream == null)
            {
                throw new ArgumentNullException(nameof(writeStream));
            }

            var completion = new TaskCompletionSource<bool>();

            if (cancellationToken.IsCancellationRequested)
            {
                completion.SetCanceled();
            }
            else
            {
                try
                {
                    WriteValue(value, writeStream, content);
                    completion.SetResult(true);
                }
                catch (Exception ex)
                {
                    completion.SetException(ex);
                }
            }

            return completion.Task;
        }

        private object ReadValue(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var contentHeaders = content?.Headers;

            if (contentHeaders != null && contentHeaders.ContentLength == 0)
            {
                return GetDefaultValueForType(type);
            }

            var encoding = SelectCharacterEncoding(contentHeaders);

            try
            {
                StreamReader reader = new StreamReader(readStream, encoding);
                string text = reader.ReadToEnd();
                return _serializer.Deserialize(text, type);
            }
            catch (Exception ex)
            {
                if (formatterLogger == null)
                {
                    throw;
                }

                formatterLogger.LogError(string.Empty, ex);

                return GetDefaultValueForType(type);
            }
        }

        private void WriteValue(object value, Stream writeStream, HttpContent content)
        {
            var encoding = SelectCharacterEncoding(content?.Headers);
            var output = _serializer.Serialize(value);
            var byteOutput = encoding.GetBytes(output);
            writeStream.Write(byteOutput, 0, byteOutput.Length);
        }
    }
}