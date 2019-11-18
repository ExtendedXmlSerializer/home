// ReSharper disable All

using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.ContentModel.Format;
using ExtendedXmlSerializer.ContentModel.Members;
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
using ISerializer = ExtendedXmlSerializer.ContentModel.ISerializer;
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
			                                            .Type<Owner>()
			                                            .Member(x => x.Element)
			                                            .Register()
			                                            .Serializer()
			                                            .Of(typeof(Serializer))
			                                            .Create();

			var instance = new Owner
			{
				Element = new DatabaseObject {Id = Guid.NewGuid(), Table = "TableName", Creted = "Loader"}
			};
			var content = container.Serialize(instance);
			var key     = new XmlReaderFactory().Get(new MemoryStream(Encoding.UTF8.GetBytes(content)));
			var owner   = Get(key);

			RestoreDatabaseObjects.Default.Execute(key);

			owner.Should().BeEquivalentTo(instance);
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

		sealed class RestoreDatabaseObjects : ICommand<XmlReader>
		{
			public static RestoreDatabaseObjects Default { get; } = new RestoreDatabaseObjects();

			RestoreDatabaseObjects() : this(Proxies.Default) {}

			readonly IProxies _proxies;

			public RestoreDatabaseObjects(IProxies proxies) => _proxies = proxies;

			public void Execute(XmlReader parameter)
			{
				var list = _proxies.Get(parameter);

				//Do processing/database calls here.
				foreach (var valueTuple in list)
				{
					valueTuple.Item3.Assign(valueTuple.Item2, new DatabaseObject
					{
						Id = valueTuple.Item1.Id, Table = valueTuple.Item1.Table, Creted = "Loader"
					});
				}
			}
		}

		interface IProxies : IParameterizedSource<object, IList<ValueTuple<Proxy, object, IMemberAccess>>> {}

		sealed class Proxies : ReferenceCache<object, IList<ValueTuple<Proxy, object, IMemberAccess>>>, IProxies
		{
			public static Proxies Default { get; } = new Proxies();

			Proxies() : base(_ => new List<ValueTuple<Proxy, object, IMemberAccess>>()) {}
		}

		sealed class Serializer : ISerializer<DatabaseObject>
		{
			readonly ISerializer           _serializer;
			readonly IMemberSerializations _members;
			readonly IContentsHistory      _history;
			readonly IProxies              _proxies;

			public Serializer(IContents contents, IMemberSerializations members)
				: this(contents.Get(typeof(Proxy).GetTypeInfo()), members, ContentsHistory.Default, Proxies.Default) {}

			public Serializer(ISerializer serializer, IMemberSerializations members, IContentsHistory history,
			                  IProxies proxies)
			{
				_serializer = serializer;
				_members    = members;
				_history    = history;
				_proxies    = proxies;
			}

			public DatabaseObject Get(IFormatReader parameter)
			{
				var owner = _history.Get(parameter)
				                    .Peek()
				                    .Current;
				var access = _members.Get(owner.GetType())
				                     .Get(parameter.Name)
				                     .Access;

				_proxies.Get(parameter.Get())
				        .Add(((Proxy)_serializer.Get(parameter), owner, access));
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

			public string Creted { get; set; }
		}
	}
#endif
}