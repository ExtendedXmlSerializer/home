using System.Linq;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core;

namespace ExtendedXmlSerializer.ExtensionModel.References
{
	sealed class ExecuteDeferredCommandsCommand : ICommand<IInnerContent>
	{
		public static ExecuteDeferredCommandsCommand Default { get; } = new ExecuteDeferredCommandsCommand();

		ExecuteDeferredCommandsCommand() : this(DeferredCommands.Default) {}

		readonly IDeferredCommands _commands;

		public ExecuteDeferredCommandsCommand(IDeferredCommands commands)
		{
			_commands = commands;
		}

		public void Execute(IInnerContent parameter)
		{
			var commands = _commands.Get(parameter.Get());
			foreach (var command in commands.ToArray())
			{
				var current = command.Get();
				if (current != null)
				{
					command.Execute(current);
					commands.Remove(command);
				}
			}
		}
	}
}