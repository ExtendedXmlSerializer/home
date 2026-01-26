using System;

namespace ExtendedXmlSerializer.ExtensionModel.Xml;

public struct ReaderPosition : IEquatable<ReaderPosition>
{
    public ReaderPosition() { }

    public ReaderPosition(int linePosition, int lineNumber)
    {
        LinePosition = linePosition;
        LineNumber = lineNumber;
    }

    public int LinePosition { get; set; }
    public int LineNumber { get; set; }

    /// <inheritdoc/>
    public override readonly bool Equals(object obj)
    {
        if (obj is ReaderPosition readerPosition)
        {
            return Equals(readerPosition);
        }

        return base.Equals(obj);
    }

    public static bool operator ==(ReaderPosition first, ReaderPosition second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(ReaderPosition first, ReaderPosition second)
    {
        return !(first == second);
    }

    /// <inheritdoc/>
    public readonly bool Equals(ReaderPosition other)
    {
        return LinePosition.Equals(other.LinePosition) && LineNumber.Equals(other.LineNumber);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            var hashCode = 47;
            hashCode = (hashCode * 53) ^ LinePosition.GetHashCode();
            hashCode = (hashCode * 53) ^ LineNumber.GetHashCode();
            return hashCode;
        }
    }
}