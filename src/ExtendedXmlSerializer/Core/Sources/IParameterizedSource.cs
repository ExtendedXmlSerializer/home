namespace ExtendedXmlSerializer.Core.Sources
{
	public interface IParameterizedSource<in TParameter, out TResult>
	{
		TResult Get(TParameter parameter);
	}
}