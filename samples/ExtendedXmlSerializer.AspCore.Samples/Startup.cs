using ExtendedXmlSerialization.AspCore.Samples.ModelConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Custom create ExtendedXmlSerializer or create it using Autofac
            SimpleSerializationToolsFactory factory = new SimpleSerializationToolsFactory();
            factory.Configurations.Add(new TestClassConfig());
            var serializer = new ExtendedXmlSerializer(factory);


            // Add framework services and add ExtendedXmlSerializer formatters
            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true; // false by default
                options.OutputFormatters.Add(new ExtendedXmlSerializerOutputFormatter(serializer));
                options.InputFormatters.Add(new ExtendedXmlSerializerInputFormatter(serializer));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
        }
    }
}
