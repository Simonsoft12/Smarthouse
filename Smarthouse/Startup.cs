using Smarthouse.Repositories.Requests;
using Smarthouse.Repositories.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Smarthouse.Models;
using System.Collections.Generic;
using System.Linq;
using Smarthouse.Static;
using Smarthouse.Repositories.Tools;

namespace Smarthouse
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LoadData();
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<IRequestsRepository, RequestsRepository>();
            services.AddTransient<IJsonRepository, JsonRepository>();
        }

        public void LoadData()
        {
            StaticData.LoadData();
        }

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
                routes.MapRoute(
                    name: "default",
                     template: "{area=Home}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
