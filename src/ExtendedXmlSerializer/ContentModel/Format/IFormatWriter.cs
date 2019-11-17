using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System.Reflection;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	/// <summary>
	/// Root-level writer object used during serialization.
	/// </summary>
	public interface IFormatWriter : IFormat, IFormatter<MemberInfo>
	{
		/// <summary>
		/// Emits the provided identity out to the destination stream.
		/// </summary>
		/// <param name="identity"></param>
		void Start(IIdentity identity);

		/// <summary>
		/// Ends the current element.
		/// </summary>
		void EndCurrent();

		/// <summary>
		/// Emits an attribute.
		/// </summary>
		/// <param name="identity">Identity of the attribute.</param>
		/// <param name="content">Content of the property.</param>
		void Content(IIdentity identity, string content);

		/// <summary>
		/// Emits content within an element.
		/// </summary>
		/// <param name="content">The content</param>
		void Content(string content);

		/// <summary>
		/// Emits content and marks it as verbatim, by use of a CDATA container.
		/// </summary>
		/// <param name="content">The content to emit.</param>
		void Verbatim(string content);
	}
}