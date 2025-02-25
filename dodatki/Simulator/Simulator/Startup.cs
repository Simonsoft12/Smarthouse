using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Applikacja.Repositories.Arduino;
using Applikacja.Repositories.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Simulator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<IArduinoResponseBuilderRepository, ArduinoResponseBuilderRepository>();
            services.AddTransient<IJsonRepository, JsonRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //  routes.MapRoute(
                //      name: "default",
                //      template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    "MyRoute",  // Route name 
                    "{ip}/{parm}",    // URL with parameters 
                    new { controller = "Home", action = "Index", ip = "", parm = "" }
                    );


            });
        }
    }
}
