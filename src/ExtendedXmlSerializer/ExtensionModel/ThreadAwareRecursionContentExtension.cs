using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;
using System.Reflection;

namespace ExtendedXmlSerializer.ExtensionModel
{
	sealed class ThreadAwareRecursionContentExtension : ISerializerExtension
	{
		public static ThreadAwareRecursionContentExtension Default { get; } = new ThreadAwareRecursionContentExtension();

		ThreadAwareRecursionContentExtension() {}

		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IRecursionContents, ThreadAwareRecursionContents>();

		void ICommand<IServices>.Execute(IServices parameter) {}

		sealed class ThreadAwareRecursionContents : IRecursionContents
		{
			readonly IRecursionContents _previous;

			public ThreadAwareRecursionContents(IRecursionContents previous) => _previous = previous;

			public IContents Get(IContents parameter) => new ThreadAwareContents(_previous.Get(parameter));
		}

		sealed class ThreadAwareContents : IContents
		{
			readonly IContents _previous;

			public ThreadAwareContents(IContents previous) => _previous = previous;

			public ISerializer Get(TypeInfo parameter)
			{
				lock (_previous)
				{
					return _previous.Get(parameter);
				}
			}
		}
	}

}
