using Microsoft.Net.Http.Headers;

namespace ExtendedXmlSerialization.AspCore
{
    internal static class MediaTypeHeaderValues
    {
        public static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml").CopyAsReadOnly();
        public static readonly MediaTypeHeaderValue TextXml = MediaTypeHeaderValue.Parse("text/xml").CopyAsReadOnly();
    }
}