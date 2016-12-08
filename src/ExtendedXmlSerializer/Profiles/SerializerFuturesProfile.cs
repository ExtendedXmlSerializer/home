using System;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Profiles
{
    /// <summary>
    /// Used to showcase the original profile with a few improvements.
    /// </summary>
    public class SerializerFuturesProfile : SerializationProfile
    {
        public static Uri Uri { get; } = new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/futures");

        public new static SerializerFuturesProfile Default { get; } = new SerializerFuturesProfile();

        SerializerFuturesProfile()
            : base(
                AutoAttributeSpecification.Default,
                Uri) {}
    }
}