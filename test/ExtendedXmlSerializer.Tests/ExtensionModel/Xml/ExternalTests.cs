using System;
using System.ComponentModel;
using System.Numerics;
using System.Xml.Serialization;
using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ExtensionModel.Xml
{
	public sealed class ExternalTests
	{
		[Fact]
		public void SceneTest()
		{
			var test = new ConfigurationContainer().Create();
			var sut = new Quaternion { Xyz = new Vector3(1f)};

			var cycled = test.Cycle(sut);
			cycled.ShouldBeEquivalentTo(sut);
		}

		/// <summary>Represents a Quaternion.</summary>
		public struct Quaternion
		{
			/// <summary>
			/// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
			/// </summary>
			[XmlIgnore]
			[EditorBrowsable(EditorBrowsableState.Never)]
			[Obsolete("Use Xyz property instead.")]
			[UsedImplicitly]
			// ReSharper disable once InconsistentNaming
			public Vector3 XYZ
			{
				get => Xyz;
				set => Xyz = value;
			}

			/// <summary>
			/// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
			/// </summary>
			public Vector3 Xyz { get; set; }
		}

	}

}
