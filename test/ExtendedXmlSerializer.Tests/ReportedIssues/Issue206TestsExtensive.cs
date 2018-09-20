using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue206TestsExtensive
	{
		[Fact]
		void Verify()
		{
			var support = new ConfigurationContainer().EnableImplicitTypingFromPublicNested<Issue206TestsExtensive>()
			                                          .EnableClassicListNaming()
			                                          .Create()
			                                          .ForTesting();

			var files = support.Deserialize<TravelFile[]>(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<ArrayOfIssue206TestsExtensive-TravelFile  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" >
   <Issue206TestsExtensive-TravelFile>
      <FileNr>543672</FileNr>
      <FileYear>2018</FileYear>
      <TotalPrice>100.3</TotalPrice>
      <CustomerID>315548</CustomerID>
      <FileStatus>Ok</FileStatus>

      <Destination>xxxx</Destination>
      <Participants>
         <Issue206TestsExtensive-Participant>
            <ParticipantId>679556</ParticipantId>
            <Name>xxxx</Name>
            <InsuranceAnnulation>false</InsuranceAnnulation>
            <InsuranceAssistance>false</InsuranceAssistance>
            <InsuranceOther>false</InsuranceOther>
            <InsuranceAnnulationDescription xsi:nil=""true""/>
            <InsuranceAssistanceDescription xsi:nil=""true""/>
            <InsuranceOtherDescription xsi:nil=""true""/>
            <ParticipantsGender>female</ParticipantsGender>
            <FirstName>xxxx</FirstName>
            <AppelationOfPart>
               <AppelationsId>MEV</AppelationsId>
               <Description>MRS</Description>
            </AppelationOfPart>
         </Issue206TestsExtensive-Participant>
         <Issue206TestsExtensive-Participant>
            <ParticipantId>679557</ParticipantId>
            <Name>xxxx</Name>
            <InsuranceAnnulation>false</InsuranceAnnulation>
            <InsuranceAssistance>false</InsuranceAssistance>
            <InsuranceOther>false</InsuranceOther>
            <InsuranceAnnulationDescription xsi:nil=""true""/>
            <InsuranceAssistanceDescription xsi:nil=""true""/>
            <InsuranceOtherDescription xsi:nil=""true""/>
            <ParticipantsGender>male</ParticipantsGender>
            <FirstName>xxxx</FirstName>
            <AppelationOfPart>
               <AppelationsId>DHR</AppelationsId>
               <Description>MR</Description>
            </AppelationOfPart>
         </Issue206TestsExtensive-Participant>
      </Participants>
      <Flights xsi:nil=""true""/>
      <AirportTransportation xsi:nil=""true""/>
      <SaldoAmount>0</SaldoAmount>
      <SaldoDate>2017-11-15T00:00:00</SaldoDate>
      <AdvanceAmount>4098.3</AdvanceAmount>
      <AdvanceDate>2017-09-26T00:00:00</AdvanceDate>
      <DateCreation>2016-05-11T16:56:50</DateCreation>
      <FileRemark>GEE/2018/000001</FileRemark>
      <CountryInfo>SPANJE</CountryInfo>
      <SupplierId>4939</SupplierId>
      <TnCompanyId xsi:nil=""true""/>
      <GroepCode xsi:nil=""true""/>
      <GroepDescription xsi:nil=""true""/>
      <Kantoor KantoorCode=""GEE"">GEEL</Kantoor>
      <FileType FileTypeCode=""NOR"">NORMAAL DOSSIER</FileType>
      <ReisType ReisTypeCode=""ACA"">AUTOCARVAKANTIE</ReisType>
      <VervoerType VervoerTypeCode=""AUB"">BUS</VervoerType>
      <TekstExtern>xxxxx</TekstExtern>
      <TekstIntern xsi:nil=""true""/>
      <AantalDeelnemers>2</AantalDeelnemers>
      <AantalNachten xsi:nil=""true""/>
      <DestinationId>300</DestinationId>
      <DestinationHotel>xxx II</DestinationHotel>
      <TotalVat>0</TotalVat>
      <CreatedBy>xxxx</CreatedBy>
      <ModifiedBy>xxxx</ModifiedBy>
      <ModifiedDate>2018-03-14T11:13:11</ModifiedDate>
      <StatusHistory>
         <Status StatusCode=""DOK"" ValidFrom=""2017-09-06T14:07:10"">IN ORDE</Status>
         <Status StatusCode=""DOK"" ValidFrom=""2016-05-11T16:56:50"">IN ORDE</Status>
         <Status StatusCode=""DOK"" ValidFrom=""2016-05-11T17:01:06"">IN ORDE</Status>
         <Status StatusCode=""DOK"" ValidFrom=""2016-05-11T17:01:28"">IN ORDE</Status>
      </StatusHistory>
      <Tickets xsi:nil=""true""/>
      <Facilities xsi:nil=""true""/>
   </Issue206TestsExtensive-TravelFile>
</ArrayOfIssue206TestsExtensive-TravelFile>")
			       /*.ShouldBeEquivalentTo(subject)*/;
			files[0].Participants.Should().HaveCount(2);
			files[0]
				.AdvanceAmount.Should()
				.Be(4098.3d);
		}

		public enum Gender
		{
			male, female
		}


		public enum FileStatus
		{
			[Description("something")]
			Ongekend,
			Ok,Request,Optie,Negatieve_request,Geannuleerd,Reischeque,Minpax,Niet_Ok
		}

		public class Appelation
		{
			public string AppelationsId { get; set; }

			public string Description { get; set; }
		}


		public class Participant
    {
        [Required]
        [Description("starts with 0")]
        public int ParticipantId { get; set; }
        [Required]
        public string Name { get; set; }
        [XmlAttribute("DateOfBirth")]
        protected string XmlstringDateOfBirth;
        [XmlIgnore]
        public DateTime? DateOfBirth
        {
            get
            {
                if (!string.IsNullOrEmpty(XmlstringDateOfBirth))
                {
                    return XmlConvert.ToDateTime(XmlstringDateOfBirth, XmlDateTimeSerializationMode.Local);
                }
                else return null;
            }
            set
            {
                if (value != null)
                    XmlstringDateOfBirth = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
            }

        }
        //public string participantType { get; set; }
        //public string travelFileId { get; set; }
        //public Boolean participantCancelled { get; set; }
        //public List<string> listArticleIds { get; set; }
        public Boolean InsuranceAnnulation { get; set; }
        public Boolean InsuranceAssistance { get; set; }
        public Boolean InsuranceOther { get; set; }
        public string InsuranceAnnulationDescription { get; set; }
        public string InsuranceAssistanceDescription { get; set; }
        public string InsuranceOtherDescription { get; set; }
        //public string appelation { get; set; }
        //nog verzekering toevoegen

        //fields unknown to the outside
        public Gender ParticipantsGender { get; set; }
        [Required]
        public string FirstName { get; set; }
        //public int Age { get; set; }
        //public string ChaimberType { get; set; }
        //public string ChaimberTypeText { get; set; }
        //public int TransportType { get; set; }
        //public SkiInfo infoSki { get; set; }
        //public Route RouteTo { get; set; }
        //public Route RouteReturn { get; set; }
        //public BoardingAndDeboardingInformation BoardingInfo { get; set; }
        public Appelation AppelationOfPart { get; set; }
        //public TravellerType TypeOfTraveller { get; set; }
        //public string RegimeText { get; set; }

        public Participant() { }

        //public Participant(string Id, string Name, DateTime? DateOfBirth, string ParticipantType, string TravelFileId,
        //                   Boolean ParticipantCancelled, List<string> ListArticleIds,
        //                   Boolean InsuranceAnnulation, Boolean InsuranceAssistance, Boolean InsuranceOther,
        //                   string InsuranceAnnulDescript, string InsuranceAssistDescript, string InsuranceOtherDescript,
        //                   string Appelation, TravellerType typeOfTraveller)
        //{
        //    this.participantId = Id;
        //    this.name = Name;
        //    this.dateOfBirth = DateOfBirth;
        //    this.participantType = ParticipantType;
        //    this.travelFileId = TravelFileId;
        //    this.participantCancelled = ParticipantCancelled;
        //    this.listArticleIds = ListArticleIds;
        //    this.insuranceAnnulation = InsuranceAnnulation;
        //    this.insuranceAssistance = InsuranceAssistance;
        //    this.insuranceOther = InsuranceOther;
        //    this.insuranceAnnulationDescription = InsuranceAnnulDescript;
        //    this.insuranceAssistanceDescription = InsuranceAssistDescript;
        //    this.insuranceOtherDescription = InsuranceOtherDescript;
        //    this.appelation = Appelation;
        //    this.TypeOfTraveller = typeOfTraveller;
        //}
    }

		public class TravelFile
		{
			// saldo en voorschot datum + saldo
			//public string travelFileId { get; set; }
			[XmlIgnore]
			public Guid Id { get; set; }

			[Required]
			public int FileNr { get; set; }

			[Required]
			public int FileYear { get; set; }

			[Required]
			public double TotalPrice { get; set; }

			[Required]
			public string CustomerID { get; set; }

			public FileStatus FileStatus { get; set; }

			//[XmlAttribute("DateDeparture")]
			//protected string XmlStringDateDeparture { get; set; }
			[XmlAttribute("DateArrivalDestination")]
			protected string XmlStringDateArrivalDestination { get; set; }

			[XmlAttribute("DateDepartureDestination")]
			protected string XmlStringDateDepartureDestination { get; set; }

			[XmlAttribute("DateReturn")]
			protected string XmlStringDateReturn { get; set; }

			[XmlAttribute("DateConfirmation")]
			protected string XmlStringDateConfirmation { get; set; }

			[Required]
			//[XmlIgnore]
			public DateTime? DateDeparture { get; set; }
			//{
			//    get
			//    {
			//        if (!string.IsNullOrEmpty(XmlStringDateDeparture))
			//        {
			//            return XmlConvert.ToDateTime(XmlStringDateDeparture, XmlDateTimeSerializationMode.Local);
			//        }
			//        else return null;
			//    }
			//    set
			//    {
			//        if (value != null)
			//            XmlStringDateDeparture = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
			//    }
			//}

			[Required]
			[XmlIgnore]
			public DateTime? DateArrivalDestination
			{
				get
				{
					if (!string.IsNullOrEmpty(XmlStringDateArrivalDestination))
					{
						return XmlConvert.ToDateTime(XmlStringDateArrivalDestination,
						                             XmlDateTimeSerializationMode.Local);
					}
					else return null;
				}
				set
				{
					if (value != null)
						XmlStringDateArrivalDestination =
							XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
				}
			}

			[Required]
			[XmlIgnore]
			public DateTime? DateDepartureDestination
			{
				get
				{
					if (!string.IsNullOrEmpty(XmlStringDateDepartureDestination))
					{
						return XmlConvert.ToDateTime(XmlStringDateDepartureDestination,
						                             XmlDateTimeSerializationMode.Local);
					}
					else return null;
				}
				set
				{
					if (value != null)
						XmlStringDateDepartureDestination =
							XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
				}
			}

			[Required]
			[XmlIgnore]
			[XmlElement]
			public DateTime? DateReturn
			{
				get
				{
					if (!string.IsNullOrEmpty(XmlStringDateReturn))
					{
						return XmlConvert.ToDateTime(XmlStringDateReturn, XmlDateTimeSerializationMode.Local);
					}
					else return null;
				}
				set
				{
					if (value != null)
						XmlStringDateReturn = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
				}
			}

			[Required]
			[XmlIgnore]
			public DateTime? DateConfirmation
			{
				get
				{
					if (!string.IsNullOrEmpty(XmlStringDateConfirmation))
					{
						return XmlConvert.ToDateTime(XmlStringDateConfirmation, XmlDateTimeSerializationMode.Local);
					}
					else return null;
				}
				set
				{
					if (value != null)
						XmlStringDateConfirmation =
							XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
				}
			}

			public string Destination { get; set; }

			[Required]
			public List<Participant> Participants { get; set; }

			public double? SaldoAmount { get; set; }
			public DateTime? SaldoDate { get; set; }
			public double? AdvanceAmount { get; set; }

			[XmlAttribute("AdvanceDate")]  protected string XmlStringAdvanceDate;
			[XmlAttribute("DateCreation")] protected string XmlStringDateCreation;

			[XmlIgnore]
			public DateTime? AdvanceDate
			{
				get
				{
					if (!string.IsNullOrEmpty(XmlStringAdvanceDate))
					{
						return XmlConvert.ToDateTime(XmlStringAdvanceDate, XmlDateTimeSerializationMode.Local);
					}
					else return null;
				}
				set
				{
					if (value != null)
						XmlStringAdvanceDate = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
				}
			}

			[Required]
			[XmlIgnore]
			public DateTime? DateCreation
			{
				get
				{
					if (!string.IsNullOrEmpty(XmlStringDateCreation))
					{
						return XmlConvert.ToDateTime(XmlStringDateCreation, XmlDateTimeSerializationMode.Local);
					}
					else return null;
				}
				set
				{
					if (value != null)
						XmlStringAdvanceDate = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.Local);
				}
			}

			public string FileRemark { get; set; }

			public string FileInternalRemark { get; set; }
			public string CountryInfo { get; set; }

			[Description("Is id of a supplier, not required")]
			public string SupplierId { get; set; }

			[Required]
			[Description("")]
			public string TnCompanyId { get; set; }

			public TravelFile() {}
		}
	}
}