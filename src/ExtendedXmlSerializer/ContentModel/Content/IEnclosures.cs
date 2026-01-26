using System.Data.Common;
using System.Reflection;
using ExtendedXmlSerializer.Core.Sources;

namespace ExtendedXmlSerializer.ContentModel.Content;

/// <summary>
/// Factory of <see cref="Enclosure{T}"/>
/// </summary>
interface IEnclosures
{
    Enclosure<T> Get<T>(IWriter<T> start, IWriter<T> body);
    
    Enclosure<T> Get<T>(IWriter<T> start, IWriter<T> body, IWriter<T> finish);
}
