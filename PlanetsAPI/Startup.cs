using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanetsAPI.Models;    // Para uso do banco de dados "in memory"
using PlanetsAPI.Services;  // Para uso do MongoDb

namespace PlanetApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the 
        //container.
        public void ConfigureServices(IServiceCollection services) {
            // Configura o uso do bando de dados "in memory"
            services.AddDbContext<PlanetContext>(opt => 
                opt.UseInMemoryDatabase("dbPlanets"));

            // Configura o uso do bando de dados MongoDb
            services.AddScoped<PlanetMService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP 
        //request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for 
                // production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // para iniciar com página HTML
            app.UseDefaultFiles();
            app.UseStaticFiles();
            // página HTML

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}