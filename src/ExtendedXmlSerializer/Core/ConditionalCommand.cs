using ExtendedXmlSerializer.Core.Specifications;

namespace ExtendedXmlSerializer.Core
{
	class ConditionalCommand<T> : ICommand<T>
	{
		readonly ISpecification<T> _specification;
		readonly ICommand<T>       _command;

		public ConditionalCommand(ICommand<T> command) : this(new ConditionalSpecification<T>(), command) {}

		public ConditionalCommand(ISpecification<T> specification, ICommand<T> command)
		{
			_specification = specification;
			_command       = command;
		}

		public void Execute(T parameter)
		{
			if (_specification.IsSatisfiedBy(parameter))
			{
				_command.Execute(parameter);
			}
		}
	}
}