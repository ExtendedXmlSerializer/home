using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Conversion;
using ExtendedXmlSerializer.Core;
using ISerializers = ExtendedXmlSerializer.ContentModel.Content.ISerializers;

namespace ExtendedXmlSerializer.ExtensionModel
{
	public sealed class CachingExtension : ISerializerExtension, ISortAware
	{
		public static CachingExtension Default { get; } = new CachingExtension();

		CachingExtension() : this(10) {}

		public CachingExtension(int sort) => Sort = sort;

		public int Sort { get; }

		public IServiceRepository Get(IServiceRepository parameter) => parameter.Decorate<IContents, CachedContents>()
		                                                                        .Decorate<ISerializers,
			                                                                        CachedSerializers>()
		                                                                        .Decorate<IConverters, CachedConverters
		                                                                        >();

		void ICommand<IServices>.Execute(IServices parameter) {}
	}
}