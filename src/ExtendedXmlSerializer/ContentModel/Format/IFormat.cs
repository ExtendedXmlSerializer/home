using System;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	public interface IFormat : ISource<object>, IIdentityStore, IDisposable {}
}