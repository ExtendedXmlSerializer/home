using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ContentModel.Conversion;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue341Tests
	{
		[Fact]
		void Verify()
		{
			const string document = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Issue341Tests-Project xmlns:exs=""https://extendedxmlserializer.github.io/v2"" xmlns:sys=""https://extendedxmlserializer.github.io/system"" xmlns=""clr-namespace:ExtendedXmlSerializer.Tests.ReportedIssues;assembly=ExtendedXmlSerializer.Tests.ReportedIssues"">
	<Documents exs:type=""sys:List[Issue341Tests-Document]"" Capacity=""4"">
		<Issue341Tests-Document Name=""New document"">
			<Items exs:type=""sys:List[Issue341Tests-Item]"" Capacity=""4"">
				<Issue341Tests-EllipseShape Id=""1"" HorizontalRadius=""200"" VerticalRadius=""100"" Left=""27.023681640625"" Top=""6.9920654296875"" />
			</Items>
		</Issue341Tests-Document>
	</Documents>
</Issue341Tests-Project>";

			var serializer = new ConfigurationContainer().EnableReferences().WithUnknownContent().Throw()
			                                             .Create();

			serializer.Deserialize<Project>(document)
			          .Documents.Only()
			          .Name.Should()
			          .Be("New document");
		}

		public class Project
		{
			public IList<Document> Documents { get; set; }
		}

		public class Document
		{
			public IList<Item> Items { get; set; }

			public string Name { get; set; }
		}

		public abstract class Item
		{
			public double Left { get; set; }
			public double Top { get; set; }
			public string Name { get; set; }
		}

		public class EllipseShape : Body
		{
			public double HorizontalRadius { get; set; }
			public double VerticalRadius { get; set; }
		}

		public abstract class Body : Item
		{
			public int Id { get; set; }
		}

		#region Supporting Classes

		public class Colors
		{
			// Copyright (c) The Avalonia Project. All rights reserved.
			// Licensed under the MIT license. See licence.md file in the project root for full license information.

			/// <summary>
			/// Gets a color with an ARGB value of #fff0f8ff.
			/// </summary>
			public static Color AliceBlue => ColorExtensions.FromUInt32(0xfff0f8ff);

			/// <summary>
			/// Gets a color with an ARGB value of #fffaebd7.
			/// </summary>
			public static Color AntiqueWhite => ColorExtensions.FromUInt32(0xfffaebd7);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00ffff.
			/// </summary>
			public static Color Aqua => ColorExtensions.FromUInt32(0xff00ffff);

			/// <summary>
			/// Gets a color with an ARGB value of #ff7fffd4.
			/// </summary>
			public static Color Aquamarine => ColorExtensions.FromUInt32(0xff7fffd4);

			/// <summary>
			/// Gets a color with an ARGB value of #fff0ffff.
			/// </summary>
			public static Color Azure => ColorExtensions.FromUInt32(0xfff0ffff);

			/// <summary>
			/// Gets a color with an ARGB value of #fff5f5dc.
			/// </summary>
			public static Color Beige => ColorExtensions.FromUInt32(0xfff5f5dc);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffe4c4.
			/// </summary>
			public static Color Bisque => ColorExtensions.FromUInt32(0xffffe4c4);

			/// <summary>
			/// Gets a color with an ARGB value of #ff000000.
			/// </summary>
			public static Color Black => ColorExtensions.FromUInt32(0xff000000);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffebcd.
			/// </summary>
			public static Color BlanchedAlmond => ColorExtensions.FromUInt32(0xffffebcd);

			/// <summary>
			/// Gets a color with an ARGB value of #ff0000ff.
			/// </summary>
			public static Color Blue => ColorExtensions.FromUInt32(0xff0000ff);

			/// <summary>
			/// Gets a color with an ARGB value of #ff8a2be2.
			/// </summary>
			public static Color BlueViolet => ColorExtensions.FromUInt32(0xff8a2be2);

			/// <summary>
			/// Gets a color with an ARGB value of #ffa52a2a.
			/// </summary>
			public static Color Brown => ColorExtensions.FromUInt32(0xffa52a2a);

			/// <summary>
			/// Gets a color with an ARGB value of #ffdeb887.
			/// </summary>
			public static Color BurlyWood => ColorExtensions.FromUInt32(0xffdeb887);

			/// <summary>
			/// Gets a color with an ARGB value of #ff5f9ea0.
			/// </summary>
			public static Color CadetBlue => ColorExtensions.FromUInt32(0xff5f9ea0);

			/// <summary>
			/// Gets a color with an ARGB value of #ff7fff00.
			/// </summary>
			public static Color Chartreuse => ColorExtensions.FromUInt32(0xff7fff00);

			/// <summary>
			/// Gets a color with an ARGB value of #ffd2691e.
			/// </summary>
			public static Color Chocolate => ColorExtensions.FromUInt32(0xffd2691e);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff7f50.
			/// </summary>
			public static Color Coral => ColorExtensions.FromUInt32(0xffff7f50);

			/// <summary>
			/// Gets a color with an ARGB value of #ff6495ed.
			/// </summary>
			public static Color CornflowerBlue => ColorExtensions.FromUInt32(0xff6495ed);

			/// <summary>
			/// Gets a color with an ARGB value of #fffff8dc.
			/// </summary>
			public static Color Cornsilk => ColorExtensions.FromUInt32(0xfffff8dc);

			/// <summary>
			/// Gets a color with an ARGB value of #ffdc143c.
			/// </summary>
			public static Color Crimson => ColorExtensions.FromUInt32(0xffdc143c);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00ffff.
			/// </summary>
			public static Color Cyan => ColorExtensions.FromUInt32(0xff00ffff);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00008b.
			/// </summary>
			public static Color DarkBlue => ColorExtensions.FromUInt32(0xff00008b);

			/// <summary>
			/// Gets a color with an ARGB value of #ff008b8b.
			/// </summary>
			public static Color DarkCyan => ColorExtensions.FromUInt32(0xff008b8b);

			/// <summary>
			/// Gets a color with an ARGB value of #ffb8860b.
			/// </summary>
			public static Color DarkGoldenrod => ColorExtensions.FromUInt32(0xffb8860b);

			/// <summary>
			/// Gets a color with an ARGB value of #ffa9a9a9.
			/// </summary>
			public static Color DarkGray => ColorExtensions.FromUInt32(0xffa9a9a9);

			/// <summary>
			/// Gets a color with an ARGB value of #ff006400.
			/// </summary>
			public static Color DarkGreen => ColorExtensions.FromUInt32(0xff006400);

			/// <summary>
			/// Gets a color with an ARGB value of #ffbdb76b.
			/// </summary>
			public static Color DarkKhaki => ColorExtensions.FromUInt32(0xffbdb76b);

			/// <summary>
			/// Gets a color with an ARGB value of #ff8b008b.
			/// </summary>
			public static Color DarkMagenta => ColorExtensions.FromUInt32(0xff8b008b);

			/// <summary>
			/// Gets a color with an ARGB value of #ff556b2f.
			/// </summary>
			public static Color DarkOliveGreen => ColorExtensions.FromUInt32(0xff556b2f);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff8c00.
			/// </summary>
			public static Color DarkOrange => ColorExtensions.FromUInt32(0xffff8c00);

			/// <summary>
			/// Gets a color with an ARGB value of #ff9932cc.
			/// </summary>
			public static Color DarkOrchid => ColorExtensions.FromUInt32(0xff9932cc);

			/// <summary>
			/// Gets a color with an ARGB value of #ff8b0000.
			/// </summary>
			public static Color DarkRed => ColorExtensions.FromUInt32(0xff8b0000);

			/// <summary>
			/// Gets a color with an ARGB value of #ffe9967a.
			/// </summary>
			public static Color DarkSalmon => ColorExtensions.FromUInt32(0xffe9967a);

			/// <summary>
			/// Gets a color with an ARGB value of #ff8fbc8f.
			/// </summary>
			public static Color DarkSeaGreen => ColorExtensions.FromUInt32(0xff8fbc8f);

			/// <summary>
			/// Gets a color with an ARGB value of #ff483d8b.
			/// </summary>
			public static Color DarkSlateBlue => ColorExtensions.FromUInt32(0xff483d8b);

			/// <summary>
			/// Gets a color with an ARGB value of #ff2f4f4f.
			/// </summary>
			public static Color DarkSlateGray => ColorExtensions.FromUInt32(0xff2f4f4f);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00ced1.
			/// </summary>
			public static Color DarkTurquoise => ColorExtensions.FromUInt32(0xff00ced1);

			/// <summary>
			/// Gets a color with an ARGB value of #ff9400d3.
			/// </summary>
			public static Color DarkViolet => ColorExtensions.FromUInt32(0xff9400d3);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff1493.
			/// </summary>
			public static Color DeepPink => ColorExtensions.FromUInt32(0xffff1493);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00bfff.
			/// </summary>
			public static Color DeepSkyBlue => ColorExtensions.FromUInt32(0xff00bfff);

			/// <summary>
			/// Gets a color with an ARGB value of #ff696969.
			/// </summary>
			public static Color DimGray => ColorExtensions.FromUInt32(0xff696969);

			/// <summary>
			/// Gets a color with an ARGB value of #ff1e90ff.
			/// </summary>
			public static Color DodgerBlue => ColorExtensions.FromUInt32(0xff1e90ff);

			/// <summary>
			/// Gets a color with an ARGB value of #ffb22222.
			/// </summary>
			public static Color Firebrick => ColorExtensions.FromUInt32(0xffb22222);

			/// <summary>
			/// Gets a color with an ARGB value of #fffffaf0.
			/// </summary>
			public static Color FloralWhite => ColorExtensions.FromUInt32(0xfffffaf0);

			/// <summary>
			/// Gets a color with an ARGB value of #ff228b22.
			/// </summary>
			public static Color ForestGreen => ColorExtensions.FromUInt32(0xff228b22);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff00ff.
			/// </summary>
			public static Color Fuchsia => ColorExtensions.FromUInt32(0xffff00ff);

			/// <summary>
			/// Gets a color with an ARGB value of #ffdcdcdc.
			/// </summary>
			public static Color Gainsboro => ColorExtensions.FromUInt32(0xffdcdcdc);

			/// <summary>
			/// Gets a color with an ARGB value of #fff8f8ff.
			/// </summary>
			public static Color GhostWhite => ColorExtensions.FromUInt32(0xfff8f8ff);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffd700.
			/// </summary>
			public static Color Gold => ColorExtensions.FromUInt32(0xffffd700);

			/// <summary>
			/// Gets a color with an ARGB value of #ffdaa520.
			/// </summary>
			public static Color Goldenrod => ColorExtensions.FromUInt32(0xffdaa520);

			/// <summary>
			/// Gets a color with an ARGB value of #ff808080.
			/// </summary>
			public static Color Gray => ColorExtensions.FromUInt32(0xff808080);

			/// <summary>
			/// Gets a color with an ARGB value of #ff008000.
			/// </summary>
			public static Color Green => ColorExtensions.FromUInt32(0xff008000);

			/// <summary>
			/// Gets a color with an ARGB value of #ffadff2f.
			/// </summary>
			public static Color GreenYellow => ColorExtensions.FromUInt32(0xffadff2f);

			/// <summary>
			/// Gets a color with an ARGB value of #fff0fff0.
			/// </summary>
			public static Color Honeydew => ColorExtensions.FromUInt32(0xfff0fff0);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff69b4.
			/// </summary>
			public static Color HotPink => ColorExtensions.FromUInt32(0xffff69b4);

			/// <summary>
			/// Gets a color with an ARGB value of #ffcd5c5c.
			/// </summary>
			public static Color IndianRed => ColorExtensions.FromUInt32(0xffcd5c5c);

			/// <summary>
			/// Gets a color with an ARGB value of #ff4b0082.
			/// </summary>
			public static Color Indigo => ColorExtensions.FromUInt32(0xff4b0082);

			/// <summary>
			/// Gets a color with an ARGB value of #fffffff0.
			/// </summary>
			public static Color Ivory => ColorExtensions.FromUInt32(0xfffffff0);

			/// <summary>
			/// Gets a color with an ARGB value of #fff0e68c.
			/// </summary>
			public static Color Khaki => ColorExtensions.FromUInt32(0xfff0e68c);

			/// <summary>
			/// Gets a color with an ARGB value of #ffe6e6fa.
			/// </summary>
			public static Color Lavender => ColorExtensions.FromUInt32(0xffe6e6fa);

			/// <summary>
			/// Gets a color with an ARGB value of #fffff0f5.
			/// </summary>
			public static Color LavenderBlush => ColorExtensions.FromUInt32(0xfffff0f5);

			/// <summary>
			/// Gets a color with an ARGB value of #ff7cfc00.
			/// </summary>
			public static Color LawnGreen => ColorExtensions.FromUInt32(0xff7cfc00);

			/// <summary>
			/// Gets a color with an ARGB value of #fffffacd.
			/// </summary>
			public static Color LemonChiffon => ColorExtensions.FromUInt32(0xfffffacd);

			/// <summary>
			/// Gets a color with an ARGB value of #ffadd8e6.
			/// </summary>
			public static Color LightBlue => ColorExtensions.FromUInt32(0xffadd8e6);

			/// <summary>
			/// Gets a color with an ARGB value of #fff08080.
			/// </summary>
			public static Color LightCoral => ColorExtensions.FromUInt32(0xfff08080);

			/// <summary>
			/// Gets a color with an ARGB value of #ffe0ffff.
			/// </summary>
			public static Color LightCyan => ColorExtensions.FromUInt32(0xffe0ffff);

			/// <summary>
			/// Gets a color with an ARGB value of #fffafad2.
			/// </summary>
			public static Color LightGoldenrodYellow => ColorExtensions.FromUInt32(0xfffafad2);

			/// <summary>
			/// Gets a color with an ARGB value of #ffd3d3d3.
			/// </summary>
			public static Color LightGray => ColorExtensions.FromUInt32(0xffd3d3d3);

			/// <summary>
			/// Gets a color with an ARGB value of #ff90ee90.
			/// </summary>
			public static Color LightGreen => ColorExtensions.FromUInt32(0xff90ee90);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffb6c1.
			/// </summary>
			public static Color LightPink => ColorExtensions.FromUInt32(0xffffb6c1);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffa07a.
			/// </summary>
			public static Color LightSalmon => ColorExtensions.FromUInt32(0xffffa07a);

			/// <summary>
			/// Gets a color with an ARGB value of #ff20b2aa.
			/// </summary>
			public static Color LightSeaGreen => ColorExtensions.FromUInt32(0xff20b2aa);

			/// <summary>
			/// Gets a color with an ARGB value of #ff87cefa.
			/// </summary>
			public static Color LightSkyBlue => ColorExtensions.FromUInt32(0xff87cefa);

			/// <summary>
			/// Gets a color with an ARGB value of #ff778899.
			/// </summary>
			public static Color LightSlateGray => ColorExtensions.FromUInt32(0xff778899);

			/// <summary>
			/// Gets a color with an ARGB value of #ffb0c4de.
			/// </summary>
			public static Color LightSteelBlue => ColorExtensions.FromUInt32(0xffb0c4de);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffffe0.
			/// </summary>
			public static Color LightYellow => ColorExtensions.FromUInt32(0xffffffe0);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00ff00.
			/// </summary>
			public static Color Lime => ColorExtensions.FromUInt32(0xff00ff00);

			/// <summary>
			/// Gets a color with an ARGB value of #ff32cd32.
			/// </summary>
			public static Color LimeGreen => ColorExtensions.FromUInt32(0xff32cd32);

			/// <summary>
			/// Gets a color with an ARGB value of #fffaf0e6.
			/// </summary>
			public static Color Linen => ColorExtensions.FromUInt32(0xfffaf0e6);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff00ff.
			/// </summary>
			public static Color Magenta => ColorExtensions.FromUInt32(0xffff00ff);

			/// <summary>
			/// Gets a color with an ARGB value of #ff800000.
			/// </summary>
			public static Color Maroon => ColorExtensions.FromUInt32(0xff800000);

			/// <summary>
			/// Gets a color with an ARGB value of #ff66cdaa.
			/// </summary>
			public static Color MediumAquamarine => ColorExtensions.FromUInt32(0xff66cdaa);

			/// <summary>
			/// Gets a color with an ARGB value of #ff0000cd.
			/// </summary>
			public static Color MediumBlue => ColorExtensions.FromUInt32(0xff0000cd);

			/// <summary>
			/// Gets a color with an ARGB value of #ffba55d3.
			/// </summary>
			public static Color MediumOrchid => ColorExtensions.FromUInt32(0xffba55d3);

			/// <summary>
			/// Gets a color with an ARGB value of #ff9370db.
			/// </summary>
			public static Color MediumPurple => ColorExtensions.FromUInt32(0xff9370db);

			/// <summary>
			/// Gets a color with an ARGB value of #ff3cb371.
			/// </summary>
			public static Color MediumSeaGreen => ColorExtensions.FromUInt32(0xff3cb371);

			/// <summary>
			/// Gets a color with an ARGB value of #ff7b68ee.
			/// </summary>
			public static Color MediumSlateBlue => ColorExtensions.FromUInt32(0xff7b68ee);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00fa9a.
			/// </summary>
			public static Color MediumSpringGreen => ColorExtensions.FromUInt32(0xff00fa9a);

			/// <summary>
			/// Gets a color with an ARGB value of #ff48d1cc.
			/// </summary>
			public static Color MediumTurquoise => ColorExtensions.FromUInt32(0xff48d1cc);

			/// <summary>
			/// Gets a color with an ARGB value of #ffc71585.
			/// </summary>
			public static Color MediumVioletRed => ColorExtensions.FromUInt32(0xffc71585);

			/// <summary>
			/// Gets a color with an ARGB value of #ff191970.
			/// </summary>
			public static Color MidnightBlue => ColorExtensions.FromUInt32(0xff191970);

			/// <summary>
			/// Gets a color with an ARGB value of #fff5fffa.
			/// </summary>
			public static Color MintCream => ColorExtensions.FromUInt32(0xfff5fffa);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffe4e1.
			/// </summary>
			public static Color MistyRose => ColorExtensions.FromUInt32(0xffffe4e1);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffe4b5.
			/// </summary>
			public static Color Moccasin => ColorExtensions.FromUInt32(0xffffe4b5);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffdead.
			/// </summary>
			public static Color NavajoWhite => ColorExtensions.FromUInt32(0xffffdead);

			/// <summary>
			/// Gets a color with an ARGB value of #ff000080.
			/// </summary>
			public static Color Navy => ColorExtensions.FromUInt32(0xff000080);

			/// <summary>
			/// Gets a color with an ARGB value of #fffdf5e6.
			/// </summary>
			public static Color OldLace => ColorExtensions.FromUInt32(0xfffdf5e6);

			/// <summary>
			/// Gets a color with an ARGB value of #ff808000.
			/// </summary>
			public static Color Olive => ColorExtensions.FromUInt32(0xff808000);

			/// <summary>
			/// Gets a color with an ARGB value of #ff6b8e23.
			/// </summary>
			public static Color OliveDrab => ColorExtensions.FromUInt32(0xff6b8e23);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffa500.
			/// </summary>
			public static Color Orange => ColorExtensions.FromUInt32(0xffffa500);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff4500.
			/// </summary>
			public static Color OrangeRed => ColorExtensions.FromUInt32(0xffff4500);

			/// <summary>
			/// Gets a color with an ARGB value of #ffda70d6.
			/// </summary>
			public static Color Orchid => ColorExtensions.FromUInt32(0xffda70d6);

			/// <summary>
			/// Gets a color with an ARGB value of #ffeee8aa.
			/// </summary>
			public static Color PaleGoldenrod => ColorExtensions.FromUInt32(0xffeee8aa);

			/// <summary>
			/// Gets a color with an ARGB value of #ff98fb98.
			/// </summary>
			public static Color PaleGreen => ColorExtensions.FromUInt32(0xff98fb98);

			/// <summary>
			/// Gets a color with an ARGB value of #ffafeeee.
			/// </summary>
			public static Color PaleTurquoise => ColorExtensions.FromUInt32(0xffafeeee);

			/// <summary>
			/// Gets a color with an ARGB value of #ffdb7093.
			/// </summary>
			public static Color PaleVioletRed => ColorExtensions.FromUInt32(0xffdb7093);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffefd5.
			/// </summary>
			public static Color PapayaWhip => ColorExtensions.FromUInt32(0xffffefd5);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffdab9.
			/// </summary>
			public static Color PeachPuff => ColorExtensions.FromUInt32(0xffffdab9);

			/// <summary>
			/// Gets a color with an ARGB value of #ffcd853f.
			/// </summary>
			public static Color Peru => ColorExtensions.FromUInt32(0xffcd853f);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffc0cb.
			/// </summary>
			public static Color Pink => ColorExtensions.FromUInt32(0xffffc0cb);

			/// <summary>
			/// Gets a color with an ARGB value of #ffdda0dd.
			/// </summary>
			public static Color Plum => ColorExtensions.FromUInt32(0xffdda0dd);

			/// <summary>
			/// Gets a color with an ARGB value of #ffb0e0e6.
			/// </summary>
			public static Color PowderBlue => ColorExtensions.FromUInt32(0xffb0e0e6);

			/// <summary>
			/// Gets a color with an ARGB value of #ff800080.
			/// </summary>
			public static Color Purple => ColorExtensions.FromUInt32(0xff800080);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff0000.
			/// </summary>
			public static Color Red => ColorExtensions.FromUInt32(0xffff0000);

			/// <summary>
			/// Gets a color with an ARGB value of #ffbc8f8f.
			/// </summary>
			public static Color RosyBrown => ColorExtensions.FromUInt32(0xffbc8f8f);

			/// <summary>
			/// Gets a color with an ARGB value of #ff4169e1.
			/// </summary>
			public static Color RoyalBlue => ColorExtensions.FromUInt32(0xff4169e1);

			/// <summary>
			/// Gets a color with an ARGB value of #ff8b4513.
			/// </summary>
			public static Color SaddleBrown => ColorExtensions.FromUInt32(0xff8b4513);

			/// <summary>
			/// Gets a color with an ARGB value of #fffa8072.
			/// </summary>
			public static Color Salmon => ColorExtensions.FromUInt32(0xfffa8072);

			/// <summary>
			/// Gets a color with an ARGB value of #fff4a460.
			/// </summary>
			public static Color SandyBrown => ColorExtensions.FromUInt32(0xfff4a460);

			/// <summary>
			/// Gets a color with an ARGB value of #ff2e8b57.
			/// </summary>
			public static Color SeaGreen => ColorExtensions.FromUInt32(0xff2e8b57);

			/// <summary>
			/// Gets a color with an ARGB value of #fffff5ee.
			/// </summary>
			public static Color SeaShell => ColorExtensions.FromUInt32(0xfffff5ee);

			/// <summary>
			/// Gets a color with an ARGB value of #ffa0522d.
			/// </summary>
			public static Color Sienna => ColorExtensions.FromUInt32(0xffa0522d);

			/// <summary>
			/// Gets a color with an ARGB value of #ffc0c0c0.
			/// </summary>
			public static Color Silver => ColorExtensions.FromUInt32(0xffc0c0c0);

			/// <summary>
			/// Gets a color with an ARGB value of #ff87ceeb.
			/// </summary>
			public static Color SkyBlue => ColorExtensions.FromUInt32(0xff87ceeb);

			/// <summary>
			/// Gets a color with an ARGB value of #ff6a5acd.
			/// </summary>
			public static Color SlateBlue => ColorExtensions.FromUInt32(0xff6a5acd);

			/// <summary>
			/// Gets a color with an ARGB value of #ff708090.
			/// </summary>
			public static Color SlateGray => ColorExtensions.FromUInt32(0xff708090);

			/// <summary>
			/// Gets a color with an ARGB value of #fffffafa.
			/// </summary>
			public static Color Snow => ColorExtensions.FromUInt32(0xfffffafa);

			/// <summary>
			/// Gets a color with an ARGB value of #ff00ff7f.
			/// </summary>
			public static Color SpringGreen => ColorExtensions.FromUInt32(0xff00ff7f);

			/// <summary>
			/// Gets a color with an ARGB value of #ff4682b4.
			/// </summary>
			public static Color SteelBlue => ColorExtensions.FromUInt32(0xff4682b4);

			/// <summary>
			/// Gets a color with an ARGB value of #ffd2b48c.
			/// </summary>
			public static Color Tan => ColorExtensions.FromUInt32(0xffd2b48c);

			/// <summary>
			/// Gets a color with an ARGB value of #ff008080.
			/// </summary>
			public static Color Teal => ColorExtensions.FromUInt32(0xff008080);

			/// <summary>
			/// Gets a color with an ARGB value of #ffd8bfd8.
			/// </summary>
			public static Color Thistle => ColorExtensions.FromUInt32(0xffd8bfd8);

			/// <summary>
			/// Gets a color with an ARGB value of #ffff6347.
			/// </summary>
			public static Color Tomato => ColorExtensions.FromUInt32(0xffff6347);

			/// <summary>
			/// Gets a color with an ARGB value of #00ffffff.
			/// </summary>
			public static Color Transparent => ColorExtensions.FromUInt32(0x00ffffff);

			/// <summary>
			/// Gets a color with an ARGB value of #ff40e0d0.
			/// </summary>
			public static Color Turquoise => ColorExtensions.FromUInt32(0xff40e0d0);

			/// <summary>
			/// Gets a color with an ARGB value of #ffee82ee.
			/// </summary>
			public static Color Violet => ColorExtensions.FromUInt32(0xffee82ee);

			/// <summary>
			/// Gets a color with an ARGB value of #fff5deb3.
			/// </summary>
			public static Color Wheat => ColorExtensions.FromUInt32(0xfff5deb3);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffffff.
			/// </summary>
			public static Color White => ColorExtensions.FromUInt32(0xffffffff);

			/// <summary>
			/// Gets a color with an ARGB value of #fff5f5f5.
			/// </summary>
			public static Color WhiteSmoke => ColorExtensions.FromUInt32(0xfff5f5f5);

			/// <summary>
			/// Gets a color with an ARGB value of #ffffff00.
			/// </summary>
			public static Color Yellow => ColorExtensions.FromUInt32(0xffffff00);

			/// <summary>
			/// Gets a color with an ARGB value of #ff9acd32.
			/// </summary>
			public static Color YellowGreen => ColorExtensions.FromUInt32(0xff9acd32);
		}

		public static class ColorExtensions
		{
			public static Color FromUInt32(uint value)
			{
				return Color.FromArgb(
				                      (byte)((value >> 24) & 0xff),
				                      (byte)((value >> 16) & 0xff),
				                      (byte)((value >> 8) & 0xff),
				                      (byte)(value & 0xff)
				                     );
			}
		}

		public sealed class ColorConverter : IConverter<Color>
		{
			public static ColorConverter Default { get; } = new ColorConverter();

			ColorConverter() {}

			public bool IsSatisfiedBy(TypeInfo parameter) => typeof(Color).GetTypeInfo()
			                                                              .IsAssignableFrom(parameter);

			public static Color FromUInt32(uint value)
			{
				return Color.FromArgb(
				                      (byte)((value >> 24) & 0xff),
				                      (byte)((value >> 16) & 0xff),
				                      (byte)((value >> 8) & 0xff),
				                      (byte)(value & 0xff)
				                     );
			}

			public Color Parse(string str)
			{
				if (str[0] == '#')
				{
					var or = 0u;

					if (str.Length == 7)
					{
						or = 0xff000000;
					}
					else if (str.Length != 9)
					{
						throw new FormatException($"Invalid color string: '{str}'.");
					}

					return FromUInt32(uint.Parse(str.Substring(1), NumberStyles.HexNumber,
					                             CultureInfo.InvariantCulture) | or);
				}
				else
				{
					var upper = str.ToUpperInvariant();
					var member = typeof(Colors).GetTypeInfo()
					                           .DeclaredProperties
					                           .FirstOrDefault(x => x.Name.ToUpperInvariant() == upper);

					if (member != null)
					{
						return (Color)member.GetValue(null);
					}
					else
					{
						throw new FormatException($"Invalid color string: '{str}'.");
					}
				}
			}

			public string Format(Color instance)
			{
				var a = instance.A.ToString("X").PadLeft(2, '0');
				var r = instance.R.ToString("X").PadLeft(2, '0');
				var g = instance.G.ToString("X").PadLeft(2, '0');
				var b = instance.B.ToString("X").PadLeft(2, '0');
				return $"#{a}{r}{g}{b}";
			}
		}

		#endregion
	}
}