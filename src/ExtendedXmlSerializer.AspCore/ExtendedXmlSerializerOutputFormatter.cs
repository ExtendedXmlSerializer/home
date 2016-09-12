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