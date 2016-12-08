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
using ExtendedXmlSerialization.Write;

namespace ExtendedXmlSerialization.Profiles
{
    public class SerializationProfileVersion20 : SerializationProfile
    {
        public static Uri Uri { get; } = new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/v2");

        public SerializationProfileVersion20() : base(AutoAttributeSpecification.Default, Uri) {}
    }

    /// <summary>
    /// Used to showcase the original profile with a few improvements.
    /// </summary>
    public class SerializerFuturesProfile : SerializationProfile
    {
        public static Uri Uri { get; } = new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/futures");
        
        public SerializerFuturesProfile()
            : base(
                AutoAttributeSpecification.Default,
                Uri) {}
        
    }

    
}