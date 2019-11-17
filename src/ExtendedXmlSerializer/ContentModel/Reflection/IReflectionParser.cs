using ExtendedXmlSerializer.Core.Parsing;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Reflection
{
	/// <summary>
	/// Used to parse text into a reflection member.
	/// </summary>
	public interface IReflectionParser : IParser<MemberInfo> {}
}