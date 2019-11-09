using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.Tests.Support;
using System;
using System.Xml.Serialization;
using Xunit;

// ReSharper disable ComplexConditionExpression

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue182Tests
	{
		[Fact]
		void Verify()
		{
			var serializer = new ConfigurationContainer()
			                 .Type<Leistungszeitraum>(c =>
			                                          {
				                                          c.Member(x => x.Tag)
				                                           .EmitWhenInstance(x => x.Typ == Leistungszeitraum
				                                                                           .LeistungszeitraumTyp.Tag);
				                                          c.Member(x => x.Monat)
				                                           .EmitWhenInstance(x => x.Typ == Leistungszeitraum
				                                                                           .LeistungszeitraumTyp
				                                                                           .Monat);
			                                          })
			                 .Create()
			                 .ForTesting();

			serializer.Assert(new Leistungszeitraum {Monat = "Hallo Welt", Typ = Leistungszeitraum.LeistungszeitraumTyp.Monat},
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue182Tests-Leistungszeitraum Typ=""Monat"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Monat>Hallo Welt</Monat></Issue182Tests-Leistungszeitraum>");
			serializer.Assert(new Leistungszeitraum {Monat = "Hallo Welt", Typ = Leistungszeitraum.LeistungszeitraumTyp.Monat, Tag = new DateTime(2018, 5, 18)},
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue182Tests-Leistungszeitraum Typ=""Monat"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Monat>Hallo Welt</Monat></Issue182Tests-Leistungszeitraum>");
			serializer.Assert(new Leistungszeitraum {Tag = new DateTime(2018, 5, 18), Typ = Leistungszeitraum.LeistungszeitraumTyp.Tag},
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue182Tests-Leistungszeitraum Typ=""Tag"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Tag>2018-05-18T00:00:00</Tag></Issue182Tests-Leistungszeitraum>");
			serializer.Assert(new Leistungszeitraum {Tag = new DateTime(2018, 5, 18), Monat = "Hallo Welt", Typ = Leistungszeitraum.LeistungszeitraumTyp.Tag},
			                  @"<?xml version=""1.0"" encoding=""utf-8""?><Issue182Tests-Leistungszeitraum Typ=""Tag"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Tag>2018-05-18T00:00:00</Tag></Issue182Tests-Leistungszeitraum>");
		}

		public class Leistungszeitraum
		{
			public enum LeistungszeitraumTyp
			{
				Tag = 1,
				Monat
			}

			[XmlAttribute]
			public LeistungszeitraumTyp Typ { get; set; }

			public DateTime Tag { get; set; }
			public string Monat { get; set; }

			//[EditorBrowsable(EditorBrowsableState.Never)]
			//public bool ShouldSerializeTag()
			//    => Typ == LeistungszeitraumTyp.Tag;

			//[EditorBrowsable(EditorBrowsableState.Never)]
			//public bool ShouldSerializeMonat()
			//    => Typ == LeistungszeitraumTyp.Monat;
		}
	}
}