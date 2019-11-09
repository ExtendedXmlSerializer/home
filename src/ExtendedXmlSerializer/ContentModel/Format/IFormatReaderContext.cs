using System;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.Core.Parsing;

namespace ExtendedXmlSerializer.ContentModel.Format
{
	interface IFormatReaderContext : IParser<MemberInfo>, IIdentityStore, IDisposable {}
}