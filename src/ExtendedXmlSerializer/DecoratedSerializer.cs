namespace ExtendedXmlSerializer
{
	/// <summary>
	/// Convenience class to decorate an existing serializer, usually used for construction.
	/// </summary>
	/// <typeparam name="TRead">The reader type.</typeparam>
	/// <typeparam name="TWrite">The writer type.</typeparam>
	public class DecoratedSerializer<TRead, TWrite> : ISerializer<TRead, TWrite>
	{
		readonly ISerializer<TRead, TWrite> _serializer;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="serializer">The existing serializer to decorate.</param>
		public DecoratedSerializer(ISerializer<TRead, TWrite> serializer) => _serializer = serializer;

		/// <inheritdoc />
		public object Deserialize(TRead reader) => _serializer.Deserialize(reader);

		/// <inheritdoc />
		public void Serialize(TWrite writer, object instance)
		{
			_serializer.Serialize(writer, instance);
		}
	}
}