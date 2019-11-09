using System.Collections.Generic;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ConverterAlterationsExtension : ISerializerExtension
	{
		public ConverterAlterationsExtension() : this(new List<IAlteration<IConverter>>()) {}

		public ConverterAlterationsExtension(ICollection<IAlteration<IConverter>> alterations)
			=> Alterations = alterations;

		public ICollection<IAlteration<IConverter>> Alterations { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=>
				parameter.RegisterInstance<IAlteration<IConverter>>(new CompositeAlteration<IConverter>(Alterations))
				         .Decorate<IConverters, AlteredConverters>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}