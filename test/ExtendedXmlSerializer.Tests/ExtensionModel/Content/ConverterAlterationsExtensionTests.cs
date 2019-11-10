using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.Tests.Support;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Content
{
	public class ConverterAlterationsExtensionTests
	{
		[Fact]
		public void Verify()
		{
			var container = new Optimizations();
			var sut       = new Alteration(container);
			var support = new SerializationSupport(new ConfigurationContainer().Alter(sut)
			                                                                   .Create());

			const float number = 4.5678f;
			var actual =
				support.Assert(number,
				               @"<?xml version=""1.0"" encoding=""utf-8""?><float xmlns=""https://extendedxmlserializer.github.io/system"">4.5678</float>");
			Assert.Equal(number, actual);

			var converter = sut.Get(FloatConverter.Default);
			var format    = converter.Format(number);
			for (var i = 0; i < 10; i++)
			{
				Assert.Same(format, converter.Format(number));
			}

			container.Clear();
			Assert.NotSame(format, converter.Format(number));
		}

		sealed class Alteration : CacheBase<IConverter, IConverter>, IAlteration<IConverter>
		{
			readonly IAlteration<IConverter> _converter;

			public Alteration(IAlteration<IConverter> converter) => _converter = converter;

			protected override IConverter Create(IConverter parameter) => _converter.Get(parameter);
		}
	}
}