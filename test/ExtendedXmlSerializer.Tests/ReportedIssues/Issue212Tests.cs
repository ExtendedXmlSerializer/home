using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.Core.Sources;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;
using XmlReader = System.Xml.XmlReader;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
#if !LEGACY

	public sealed class Issue212Tests
	{
		[Fact]
		void Verify()
		{
			var container = new ConfigurationContainer().EnableReaderContext()
			                                            .ConfigureType<Owner>()
			                                            .Member(x => x.Element)
			                                            .Register(typeof(Serializer))
			                                            .Create();

			var instance = new Owner {Element = new DatabaseObject {Id = Guid.NewGuid(), Table = "TableName"}};
			var content  = container.Serialize(instance);
			var key      = new XmlReaderFactory().Get(new MemoryStream(Encoding.UTF8.GetBytes(content)));
			var owner    = Get(key);
			var list     = Proxies.Default.Get(key);

			// Do processing/database calls here.
			foreach (var tuple in list)
			{
				tuple.Item1.Element = new DatabaseObject {Id = tuple.Item2.Id, Table = tuple.Item2.Table};
			}

			owner.ShouldBeEquivalentTo(instance);
			owner.Element.Table.Should()
			     .Be("TableName");

			Owner Get(XmlReader reader)
			{
				using (reader)
				{
					return (Owner)container.Deserialize(reader);
				}
			}
		}

		interface IProxies : IParameterizedSource<object, IList<ValueTuple<Owner, Proxy>>> {}

		sealed class Proxies : ReferenceCache<object, IList<ValueTuple<Owner, Proxy>>>, IProxies
		{
			public static Proxies Default { get; } = new Proxies();

			Proxies() : base(_ => new List<ValueTuple<Owner, Proxy>>()) {}
		}

		sealed class Serializer : ISerializer<DatabaseObject>
		{
			readonly ISerializer      _serializer;
			readonly IContentsHistory _history;
			readonly IProxies         _proxies;

			public Serializer(IContents contents)
				: this(contents.Get(typeof(Proxy).GetTypeInfo()), ContentsHistory.Default, Proxies.Default) {}

			public Serializer(ISerializer serializer, IContentsHistory history, IProxies proxies)
			{
				_serializer = serializer;
				_history    = history;
				_proxies    = proxies;
			}

			public DatabaseObject Get(IFormatReader parameter)
			{
				var owner = _history.Get(parameter)
				                    .Peek()
				                    .Current.To<Owner>();
				var proxy = (Proxy)_serializer.Get(parameter);
				_proxies.Get(parameter.Get())
				        .Add((owner, proxy));
				return null;
			}

			public void Write(IFormatWriter writer, DatabaseObject instance)
			{
				_serializer.Write(writer, new Proxy {Id = instance.Id, Table = instance.Table});
			}
		}

		struct Proxy
		{
			public Guid Id { get; set; }

			public string Table { get; set; }
		}

		sealed class Owner
		{
			public DatabaseObject Element { get; set; }
		}

		sealed class DatabaseObject
		{
			public Guid Id { get; set; }

			public string Table { get; set; }
		}
	}
#endif
}