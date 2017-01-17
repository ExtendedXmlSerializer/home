namespace ExtendedXmlSerialization.Performance.Tests.Legacy
{
    public interface IPropertyEncryption
    {
        string Encrypt(string value);
        string Decrypt(string value);
    }
}
