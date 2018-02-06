using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ReflectionModel;
using System.IO;
using Xunit;

namespace ExtendedXmlSerializer.Tests.Configuration
{
	public sealed class ContainerTests
	{
		[Fact]
		public void Verify()
		{
			var temp = new ConfigurationContainer().Get();
			var serializer = temp.Get<int>();

			using (var stream = DefaultActivators.Default.New<MemoryStream>())
			{
				serializer.Execute(new Input<int>(stream, 6776));


				stream.Seek(0, SeekOrigin.Begin);
				//var result = new StreamReader(stream).ReadToEnd();
				//throw new InvalidOperationException(result);
			}
		}
	}
}
