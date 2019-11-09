using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Content;

namespace ExtendedXmlSerializer.ContentModel.Members
{
	sealed class MemberedContents : IContents
	{
		readonly IInstanceMemberSerializations _instances;
		readonly IInnerContentServices         _services;

		public MemberedContents(IInstanceMemberSerializations instances, IInnerContentServices services)
		{
			_instances = instances;
			_services  = services;
		}

		public ISerializer Get(TypeInfo parameter)
		{
			var members = _instances.Get(parameter);
			var reader =
				_services.Create(parameter,
				                 new MemberInnerContentHandler(_instances.Get(parameter), _services, _services));
			var result = new Serializer(reader, new MemberListWriter(members));
			return result;
		}
	}
}