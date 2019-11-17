using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;
using System;
using System.Xml;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	/// <summary>
	/// Base container that holds identities and the native root objects (e.g. <see cref="XmlReader"/>, or <see cref="XmlWriter"/>) during deserialization or serialization.
	/// </summary>
	public interface IFormat : ISource<object>, IIdentityStore, IDisposable {}
}