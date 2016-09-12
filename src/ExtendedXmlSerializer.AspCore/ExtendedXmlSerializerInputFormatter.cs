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