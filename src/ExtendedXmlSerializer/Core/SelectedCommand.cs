using System;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.Core
{
	class SelectedCommand<T> : ICommand<T>
	{
		readonly IParameterizedSource<T, ICommand<T>> _selector;

		public SelectedCommand(IParameterizedSource<T, ICommand<T>> selector) => _selector = selector;

		public void Execute(T parameter) => _selector.Get(parameter)
		                                             ?.Execute(parameter);
	}

	sealed class Command<T> : ICommand<T>
	{
		readonly Action<T> _action;

		public Command(Action<T> action) => _action = action;

		public void Execute(T parameter)
		{
			_action(parameter);
		}
	}
}