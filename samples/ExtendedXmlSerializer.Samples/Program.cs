﻿// MIT License
//
// Copyright (c) 2016-2018 Wojciech Nagórski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.Samples.CustomSerializator;
using ExtendedXmlSerializer.Samples.Dictianary;
using ExtendedXmlSerializer.Samples.Encrypt;
using ExtendedXmlSerializer.Samples.Extensibility;
using ExtendedXmlSerializer.Samples.MigrationMap;
using ExtendedXmlSerializer.Samples.ObjectReference;
using ExtendedXmlSerializer.Samples.Simple;
using ExtendedXmlSerializer.Samples.Upgrade;
using System;

namespace ExtendedXmlSerializer.Samples
{
	using FluentApi;
	using Generics;
	using Parametrized;

	public class Program
	{
		public static void PrintHeader(string title)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(title);
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static void Main(string[] args)
		{
			FluentApiSamples.Run();

			Introduction.Run.Default.Execute(null);
			Converters.Default.Execute(null);
			OptimizedNamespaces.Default.Execute(null);
			ImplicitTypes.Default.Execute(null);
			AutoFormatting.Default.Execute(null);
			PrivateConstructors.Default.Execute(null);
			ParameterizedContent.Default.Execute(null);
			Tuples.Default.Execute(null);
			AttachedProperties.Default.Execute(null);
			MarkupExtensions.Default.Execute(null);
			Example.Default.Execute(null);
			VerbatimContent.Default.Execute(null);

			SimpleSamples.Run();
			Serialization.Default.Execute(null);
			Overrides.Default.Execute(null);
			DictianarySamples.Run();
			FluentApiSamples.Run();

			CustomSerializatorSamples.RunSimpleConfig();
			// CustomSerializatorSamples.RunAutofacConfig();
			MigrationMapSamples.RunSimpleConfig();
			//MigrationMapSamples.RunAutofacConfig();
			ObjectReferenceSamples.RunSimpleConfig();
			// ObjectReferenceSamples.RunAutofacConfig();
			EncryptSamples.RunSimpleConfig();
            //EncryptSamples.RunAutofacConfig();

            ParametrizedConstructors.SerializeAndDeserialize();
            GenericSerializer.SerializeAndDeserialize();

            Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}