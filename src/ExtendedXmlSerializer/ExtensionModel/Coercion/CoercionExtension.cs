using System.Collections.Generic;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.Coercion
{
	sealed class CoercionExtension : ISerializerExtension
	{
		public CoercionExtension()
			: this(new HashSet<ICoercer>
				       {FrameworkCoercer.Default, TypeCoercer.Default, TypeMetadataCoercer.Default}) {}

		public CoercionExtension(ICollection<ICoercer> coercers)
		{
			Coercers = coercers;
		}

		public ICollection<ICoercer> Coercers { get; }

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterInstance<IEnumerable<ICoercer>>(Coercers)
			            .Register<ICoercers, Coercers>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}