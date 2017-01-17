// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ExtendedXmlSerialization.AspCore.Samples.Model;
using ExtendedXmlSerialization.AspCore.Samples.ModelConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExtendedXmlSerialization.Autofac;

namespace ExtendedXmlSerialization.AspCore.Samples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }
        
//////////////////////////////////////////////////////////////////////////////
/// Autofac configuration 
//////////////////////////////////////////////////////////////////////////////
        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the Configure method, below.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection.
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true; // false by default

                //Resolve ExtendedXmlSerializer
                IExtendedXmlSerializer serializer = ApplicationContainer.Resolve<IExtendedXmlSerializer>();

                //Add ExtendedXmlSerializer's formatter
                options.OutputFormatters.Add(new ExtendedXmlSerializerOutputFormatter(serializer));
                options.InputFormatters.Add(new ExtendedXmlSerializerInputFormatter(serializer));
            });

            // Create the container builder.
            var builder = new ContainerBuilder();

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.
            builder.Populate(services);
            builder.RegisterModule<AutofacExtendedXmlSerializerModule>();
            builder.RegisterType<TestClassConfig>().As<ExtendedXmlSerializerConfig<TestClass>>().SingleInstance();
            ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);
        }

//////////////////////////////////////////////////////////////////////////////
/// Manual configuration
//////////////////////////////////////////////////////////////////////////////

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the Configure method, below.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            // Custom create ExtendedXmlSerializer
//            SimpleSerializationToolsFactory factory = new SimpleSerializationToolsFactory();
//            factory.Configurations.Add(new TestClassConfig());
//            IExtendedXmlSerializer serializer = new ExtendedXmlSerializer(factory);
//
//            // Add services to the collection.
//            services.AddMvc(options =>
//            {
//                options.RespectBrowserAcceptHeader = true; // false by default
//
//                //Add ExtendedXmlSerializer's formatter
//                options.OutputFormatters.Add(new ExtendedXmlSerializerOutputFormatter(serializer));
//                options.InputFormatters.Add(new ExtendedXmlSerializerInputFormatter(serializer));
//            });
//        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            if (ApplicationContainer != null)
            {
                // If you want to dispose of resources that have been resolved in the
                // application container, register for the "ApplicationStopped" event.
                appLifetime.ApplicationStopped.Register(ApplicationContainer.Dispose);
            }
        }
    }
}
