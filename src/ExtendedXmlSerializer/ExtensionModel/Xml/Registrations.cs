using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Xml;

namespace ExtendedXmlSerializer.ExtensionModel.Xml
{
	sealed class Registrations : Items<IConfiguration>
	{
		public static Registrations Default { get; } = new Registrations();

		Registrations() : this(Register.For(XmlConvert.ToBoolean, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToChar, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToSByte, XmlConvert.ToString),
		                       Register.For(Convert.FromBase64String, Convert.ToBase64String),
		                       Register.For(XmlConvert.ToByte, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToInt16, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToUInt16, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToInt32, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToUInt32, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToInt64, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToUInt64, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToSingle, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToDouble, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToDecimal, XmlConvert.ToString),
		                       Register.For(x => XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.RoundtripKind),
		                                    x => XmlConvert.ToString(x, XmlDateTimeSerializationMode.RoundtripKind)),
		                       Register.For(XmlConvert.ToDateTimeOffset, XmlConvert.ToString),
		                       Register.For(Self<string>.Default.Get, Self<string>.Default.Get),
		                       Register.For(XmlConvert.ToGuid, XmlConvert.ToString),
		                       Register.For(XmlConvert.ToTimeSpan, XmlConvert.ToString),
		                       Register.For(s => new Uri(s), uri => uri.ToString())) {}

		public Registrations(params IConfiguration[] items) : base(items) {}
	}
}