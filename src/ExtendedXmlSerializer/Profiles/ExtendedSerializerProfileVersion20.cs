// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System;
using System.Collections.Generic;
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Profiles
{
	public class ExtendedSerializerProfileVersion20 : ExtendedSerializationProfileBase
	{
		public static ExtendedSerializerProfileVersion20 Default { get; } = new ExtendedSerializerProfileVersion20();
		ExtendedSerializerProfileVersion20() : base(new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/v2")) {}

		public override IExtendedXmlSerializer Create()
		{
			var host = new SerializationToolsFactoryHost();
			var services = new HashSet<object>();
			var writings = new WritingFactory(host, services);
			var plan = AutoAttributeWritePlanComposer.Default.Compose();
			var result = new ExtendedXmlSerializer(host, services, new AssignmentFactory(host), writings,
			                                       new Serializer(plan, writings));
			return result;
		}
	}
}