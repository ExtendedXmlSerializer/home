namespace ExtendedXmlSerializer
{
	public interface ISerializer<in TRead, in TWrite>
	{
		object Deserialize(TRead reader);

		void Serialize(TWrite writer, object instance);
	}
}