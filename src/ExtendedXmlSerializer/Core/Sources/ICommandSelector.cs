namespace ExtendedXmlSerializer.Core.Sources
{
	interface ICommandSelector<in T> : IParameterizedSource<T, ICommand<T>> {}
}