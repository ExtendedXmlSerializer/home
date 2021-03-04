using System.IO;
using System.Xml;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Light.GuardClauses;

namespace VweCore.Xml
{
    public static class XmlSerializer
    {
        private static readonly XmlWriterSettings WriteIndentedSettings = new () { Indent = true, IndentChars = "    " };

        public static IExtendedXmlSerializer CreateDefaultSerializer() =>
            new ConfigurationContainer().UseOptimizedNamespaces()
                                        .EnableParameterizedContentWithPropertyAssignments()
                                        .UseAutoFormatting()
                                        .Type<Node>()
                                        .EnableReferences(node => node.InternalId)
                                        .Type<HallwayNode>()
                                        .EnableReferences(hallwayNode => hallwayNode.InternalId)
                                        .Type<TagNode>()
                                        .EnableReferences(tagNode => tagNode.InternalId)
                                        .Type<MarkerPoint>()
                                        .EnableReferences(markerPoint => markerPoint.InternalId)
                                        .Type<StorageRow>()
                                        .EnableReferences(row => row.InternalId)
                                        .Type<StorageLocation>()
                                        .EnableReferences(virtualPoint => virtualPoint.InternalId)
                                        .Type<VirtualPoint>()
                                        .EnableReferences(location => location.InternalId)
                                        .Type<NodeLink>()
                                        .EnableReferences(nodeLink => nodeLink.InternalId)
                                        .Create();

        public static string SerializeIndented(this IExtendedXmlSerializer serializer, object instance)
        {
            serializer.MustNotBeNull(nameof(serializer));
            return serializer.Serialize(WriteIndentedSettings, instance);
        }

        public static void SerializeIndented(this IExtendedXmlSerializer serializer, string fileName, object instance)
        {
            serializer.MustNotBeNull(nameof(serializer));
            using var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            serializer.Serialize(WriteIndentedSettings, stream, instance);
        }
    }
}