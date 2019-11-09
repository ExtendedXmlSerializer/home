using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	public interface IFormatWriter : IFormat, IFormatter<MemberInfo>
	{
		void Start(IIdentity identity);

		void EndCurrent();

		void Content(IIdentity property, string content);

		void Content(string content);

		void Verbatim(string content);
	}
}