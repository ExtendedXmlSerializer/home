using ExtendedXmlSerializer.ExtensionModel.Xml;

namespace ExtendedXmlSerializer.ContentModel.Format;

public interface IRegressableReader : IFormatReader
{
    public bool IsRegressable { get; }

    public bool SetPosition(ReaderPosition position);

    // Null if none
    public ReaderPosition GetPosition();
}