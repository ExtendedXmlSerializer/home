using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ISerializers = ExtendedXmlSerializer.ContentModel.Content.ISerializers;

namespace ExtendedXmlSerializer.ExtensionModel
{
	/// <summary>
	/// Used to cache commonly used components that are selected during serialization.
	/// </summary>
	public sealed class CachingExtension : ISerializerExtension, ISortAware
	{
		/// <summary>
		/// The default instance with a default sort of 10.
		/// </summary>
		public static CachingExtension Default { get; } = new CachingExtension();

		CachingExtension() : this(10) {}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="sort">The instance sort value.</param>
		public CachingExtension(int sort) => Sort = sort;

		/// <inheritdoc />
		public int Sort { get; }

		/// <inheritdoc />
		public IServiceRepository Get(IServiceRepository parameter)
			=> parameter.Decorate<IContents, CachedContents>()
			            .Decorate<ISerializers, CachedSerializers>()
			            .Decorate<IConverters, CachedConverters>();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}