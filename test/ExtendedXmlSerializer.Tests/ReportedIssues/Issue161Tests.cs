using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel;
using ExtendedXmlSerializer.Tests.Support;
using System.Runtime.Serialization;
using Xunit;
using Type = System.Type;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue161Tests
	{
		[Fact]
		public void Verify()
		{
			var serializer = IntegrationExtensions.Register(new ConfigurationContainer(), SerializationSurrogateProvider.Default)
			                                             .Create()
			                                             .ForTesting();
			serializer.Assert(new Subject{ Message = "Surrogates in the hizzy, dawg." }, @"<?xml version=""1.0"" encoding=""utf-8""?><Issue161Tests-Subject xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests""><Message>Hello world from Surrogate: Surrogates in the hizzy, dawg.</Message></Issue161Tests-Subject>");
		}

		sealed class Subject
		{
			public string Message { get; set; }
		}

		sealed class Surrogate
		{
			public string Message { get; set; }
		}

		sealed class SerializationSurrogateProvider : ISerializationSurrogateProvider
		{
			public static SerializationSurrogateProvider Default { get; } = new SerializationSurrogateProvider();
			SerializationSurrogateProvider() {}

			public object GetDeserializedObject(object obj, Type targetType)
			{
				var message = obj.To<Surrogate>()
				                 .Message;
				var result = new Subject{ Message =  message};
				return result;
			}

			public object GetObjectToSerialize(object obj, Type targetType)
			{
				var message = obj.To<Subject>()
				   .Message;
				var result = new Surrogate { Message = $"Hello world from Surrogate: {message}"};
				return result;
			}

			public Type GetSurrogateType(Type type) => type == typeof(Subject) ? typeof(Surrogate) : null;
		}
	}
}
