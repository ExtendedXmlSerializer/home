using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue199Tests
	{
		[Fact]
		void Verify()
		{
			var registry = new PrefixRegistryExtension(new Dictionary<Type, string>
				                                            {{typeof(PatientVerificationRequest), "q"}});
			var container = new ConfigurationContainer().Extend(registry)
			                                            // Totally cheating here. Put formatter logic to get what you want:
			                                            .Register(x => "2018-08-07+10:00",
			                                                      x => new DateTime(2018, 8, 7, 10, 0, 0))
			                                            .InspectingType<PatientVerificationRequest>()
			                                            .Create()
			                                            .ForTesting();

			container.Assert(new PatientVerificationRequest {EarliestDateOfService = DateTime.Today},
			                 @"<?xml version=""1.0"" encoding=""utf-8""?><q:patientVerificationRequest OPVTypeCde=""PVM"" earliestDateOfService=""2018-08-07+10:00"" xmlns:q=""http://hic.gov.au/hiconline/hiconline/version-4"" />");
		}

		[System.CodeDom.Compiler.GeneratedCodeAttribute("XmlSchemaClassGenerator", "1.0.0.0")]
		[System.SerializableAttribute()]
		[System.Xml.Serialization.XmlTypeAttribute("PatientVerificationRequest", Namespace =
			"http://hic.gov.au/hiconline/hiconline/version-4")]
		[System.Diagnostics.DebuggerStepThroughAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlRootAttribute("patientVerificationRequest", Namespace =
			"http://hic.gov.au/hiconline/hiconline/version-4")]
		[System.Xml.Serialization.XmlIncludeAttribute(typeof(ReferencePatientVerificationRequest))]
		public partial class PatientVerificationRequest
		{
			/// <summary>
			/// </summary>
			[System.Xml.Serialization.XmlAttributeAttribute("OPVTypeCde", Form =
				System.Xml.Schema.XmlSchemaForm.Unqualified)]
			public OPVTypeCdeEnum OPVTypeCde { get; set; }

			/// <summary>
			/// </summary>
			[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
			[System.Xml.Serialization.XmlAttributeAttribute("earliestDateOfService",
				Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date")]
			public System.DateTime EarliestDateOfServiceValue { get; set; }

			/// <summary>
			/// <para xml:lang="de">Ruft einen Wert ab, der angibt, ob die EarliestDateOfService-Eigenschaft spezifiziert ist, oder legt diesen fest.</para>
			/// <para xml:lang="en">Gets or sets a value indicating whether the EarliestDateOfService property is specified.</para>
			/// </summary>
			[System.Xml.Serialization.XmlIgnoreAttribute()]
			[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
			public bool EarliestDateOfServiceValueSpecified { get; set; }

			/// <summary>
			/// </summary>
			[System.Xml.Serialization.XmlIgnoreAttribute()]
			public System.Nullable<System.DateTime> EarliestDateOfService
			{
				get
				{
					if (this.EarliestDateOfServiceValueSpecified)
					{
						return this.EarliestDateOfServiceValue;
					}
					else
					{
						return null;
					}
				}
				set
				{
					this.EarliestDateOfServiceValue          = value.GetValueOrDefault();
					this.EarliestDateOfServiceValueSpecified = value.HasValue;
				}
			}
		}
		public enum OPVTypeCdeEnum
		{
			PVM
		}

		public class ReferencePatientVerificationRequest {}
	}
}