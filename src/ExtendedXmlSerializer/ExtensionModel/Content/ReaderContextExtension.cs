using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ExtensionModel.Content
{
	sealed class ReaderContextExtension : ISerializerExtension
	{
		public static ReaderContextExtension Default { get; } = new ReaderContextExtension();

		ReaderContextExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IAlteration<IInnerContentHandler>, Wrapper>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class Wrapper : IAlteration<IInnerContentHandler>
		{
			readonly IAlteration<IInnerContentHandler> _handler;

			public Wrapper(IAlteration<IInnerContentHandler> handler)
			{
				_handler = handler;
			}

			public IInnerContentHandler Get(IInnerContentHandler parameter) => new Handler(_handler.Get(parameter));

			sealed class Handler : IInnerContentHandler
			{
				readonly IContentsHistory     _contexts;
				readonly IInnerContentHandler _handler;

				public Handler(IInnerContentHandler handler) : this(ContentsHistory.Default, handler) {}

				public Handler(IContentsHistory contexts, IInnerContentHandler handler)
				{
					_contexts = contexts;
					_handler  = handler;
				}

				public bool IsSatisfiedBy(IInnerContent parameter)
				{
					var contexts = _contexts.Get(parameter.Get());
					contexts.Push(parameter);
					var result = _handler.IsSatisfiedBy(parameter);
					contexts.Pop();
					return result;
				}
			}
		}
	}
}