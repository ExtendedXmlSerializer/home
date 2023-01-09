using FluentAssertions;
using System.Collections.Generic;
using VweCore;
using VweCore.Xml;
using Xunit;
using Xunit.Abstractions;

namespace ExtendedXmlSerializer.Tests.ReportedIssues.Issue502
{
    public sealed class Issue502Tests
    {
        private readonly ITestOutputHelper _output;

        public Issue502Tests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void SerializeMap()
        {
            var serializer = XmlSerializer.CreateDefaultSerializer();
            var map = TestMapFactory.CreateMapWithDefaultData();

            var xml = serializer.SerializeIndented(map);
            _output.WriteLine(xml);
            var deserializedMap = serializer.Deserialize<Map>(xml);

            deserializedMap.Should().BeEquivalentTo(map, config => config.IgnoringCyclicReferences());
            var node1 = deserializedMap.Nodes[0];
            var node2 = deserializedMap.Nodes[1];
            node1.NodeLinks[0].Node2.Should().BeSameAs(node2);
            node2.NodeLinks[0].Node1.Should().BeSameAs(node1);
        }

        [Fact]
        public void SerializeStorageLocation()
        {
            var serializer = XmlSerializer.CreateDefaultSerializer();
            var storageLocation = new StorageLocation();

            var xml = serializer.SerializeIndented(storageLocation);
            _output.WriteLine(xml);
            var deserializedStorageLocation = serializer.Deserialize<StorageLocation>(xml);

            deserializedStorageLocation.Should().BeEquivalentTo(storageLocation, config => config.IgnoringCyclicReferences());
        }

        [Fact]
        public void SerializeMapWithStorageLocationAndStorageRow()
        {
            var serializer = XmlSerializer.CreateDefaultSerializer();

            var storageLocation = new StorageLocation();
            var storageRow      = new StorageRow { StorageLocations = { storageLocation } };
            storageLocation.StorageRow = storageRow;
            var map = new Map();
            map.Add(storageRow);
            map.Add(storageLocation);

            var xml = serializer.SerializeIndented(map);
            _output.WriteLine(xml);
            var deserializedMap = serializer.Deserialize<Map>(xml);

            deserializedMap.Should().BeEquivalentTo(map, config => config.IgnoringCyclicReferences());
        }

        [Fact]
        public void SerializeStorageRow()
        {
            var serializer = XmlSerializer.CreateDefaultSerializer();

            var markerPoint     = new MarkerPoint();
            var storageLocation = new StorageLocation { MarkerPoints = new List<MarkerPoint> {markerPoint}};
            markerPoint.StorageLocation = storageLocation;
            var storageRow = new StorageRow {StorageLocations = {storageLocation}};
            storageLocation.StorageRow = storageRow;

            var xml = serializer.SerializeIndented(storageRow);
            _output.WriteLine(xml);
            var deserializedStorageRow = serializer.Deserialize<StorageRow>(xml);

            deserializedStorageRow.Should().BeEquivalentTo(storageRow, config => config.IgnoringCyclicReferences());
        }


        [Fact]
        public void SerializeSmallMap()
        {
            var serializer = XmlSerializer.CreateDefaultSerializer();

            var markerPoint     = new MarkerPoint();
            var storageLocation = new StorageLocation { MarkerPoints = new List<MarkerPoint> {markerPoint}};
            markerPoint.StorageLocation = storageLocation;
            var storageRow = new StorageRow { StorageLocations = { storageLocation } };
            storageLocation.StorageRow = storageRow;
            var map = new Map {Name = "test"};
            map.Add(storageRow);
            map.Add(storageLocation);
            map.Add(markerPoint);

            var xml = serializer.SerializeIndented(map);
            _output.WriteLine(xml);

            var deserializer = XmlSerializer.CreateDefaultSerializer();
            var deserializedMap = deserializer.Deserialize<Map>(xml);

            deserializedMap.Should().BeEquivalentTo(map, config => config.IgnoringCyclicReferences());
        }

        [Fact]
        public void SerializeHomeNodeNotMarkerPoint()
        {
            var serializer = XmlSerializer.CreateDefaultSerializer();

            var somePoint = new HallwayNode {Id = 3};

            var storageLocation = new StorageLocation();
            var markerPoint     = new MarkerPoint { StorageLocation = storageLocation};
            storageLocation.MarkerPoints.Add(markerPoint);
            var storageRow = new StorageRow { StorageLocations = { storageLocation } };
            storageLocation.StorageRow = storageRow;
            var map = new Map();
            map.Add(somePoint);
            map.Add(storageRow);
            map.Add(storageLocation);
            map.Add(markerPoint);

            var xml = serializer.SerializeIndented(map);
            _output.WriteLine(xml);
            var deserializedMap = serializer.Deserialize<Map>(xml);

            deserializedMap.Should().BeEquivalentTo(map, config => config.IgnoringCyclicReferences());
        }
    }
}