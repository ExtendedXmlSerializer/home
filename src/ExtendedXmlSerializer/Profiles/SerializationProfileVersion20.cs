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
    public class SerializationProfileVersion20 : SerializationProfile // TODO: Extend.
    {
        public static SerializationProfileVersion20 Default { get; } = new SerializationProfileVersion20();
        private SerializationProfileVersion20() : this(new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/v2")) {}
        private SerializationProfileVersion20(Uri uri) : base(AutoAttributeSpecification.Default, uri) {}
        
        /*public override ISerialization New()
        {
           /* /*var host = new SerializationToolsFactoryHost();
            var services = new HashSet<object>();
            var factory = new WritingFactory(host, services, _locator);
            var plan = AutoAttributeWritePlanComposer.Default.Compose();
            var serializer = new Serializer(plan, factory);
            var result = new Serialization(host, services, factory, serializer);
            return result;#2#
            var host = new SerializationToolsFactoryHost();
            var services = new ServiceRepository(_formatter);
            var writeExtensions = new OrderedSet<IExtension>();
            var readExtensions = new OrderedSet<IExtension>();
            
            var factory = new WritingFactory(host, _locator, services, new CompositeExtension(writeExtensions));
            /*var writeExtensions = new OrderedSet<IWritingExtension>();
            var extension = new CompositeWritingExtension(writeExtensions);
            var plan = new CachedWritePlan(new ExtensionEnabledWritePlan(new WritePlanComposer(AutoAttributePlanSelector.Default).Compose(), extension));#2#
            
            // var alteration = new CompositeAlteration<IWritePlan>(new EnableExtensionPlanAlteration(extension), CacheWritePlanAlteration.Default);
            var plan = new WritePlanComposer(new PlanSelector(AutoAttributeSpecification.Default)).Compose();
            var serializer = new Serializer(plan, factory);
            var result = new Serialization(host, serializer, services, readExtensions, writeExtensions);
            writeExtensions.Add(new DefaultWritingExtensions(result));
            return result;#1#
        }*/
    }

    /// <summary>
    /// Used to showcase the original profile with a few improvements.
    /// </summary>
    public class SerializerFuturesProfile : SerializationProfile
    {
        public static SerializerFuturesProfile Default { get; } = new SerializerFuturesProfile();

        SerializerFuturesProfile()
            : base(
                AutoAttributeSpecification.Default,
                new Uri("https://github.com/wojtpl2/ExtendedXmlSerializer/futures")) {}
        
    }

    
}