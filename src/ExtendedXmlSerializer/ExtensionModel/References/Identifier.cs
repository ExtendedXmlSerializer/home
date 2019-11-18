namespace ExtendedXmlSerializer.ExtensionModel.References
{
	readonly struct Identifier
	{
		public Identifier(uint uniqueId, IEntity entity = null)
		{
			UniqueId = uniqueId;
			Entity   = entity;
		}

		public uint UniqueId { get; }
		public IEntity Entity { get; }
	}
}