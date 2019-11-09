using System;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel
{
	class DecorateAlteration<TFrom, TTo, TParameter, TResult> : IAlteration<IServiceRepository>
		where TFrom : IParameterizedSource<TParameter, TResult>
		where TTo : TFrom
	{
		readonly Func<TFrom, TTo, TFrom> _factory;

		public DecorateAlteration(Func<TFrom, TTo, TFrom> factory) => _factory = factory;

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.RegisterWithDependencies<TTo>()
			            .Decorate<TFrom>(Decorate);

		TFrom Decorate(IServiceProvider services, TFrom current) => _factory(current, services.Get<TTo>());
	}
}