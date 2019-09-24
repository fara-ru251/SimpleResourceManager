using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using ManagerAPI.UI.Models.ActorProviders;
using ManagerAPI.UI.Models.ActorSystemConfig;
using ManagerAPI.UI.Models.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ManagerAPI.UI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton(_ => ActorSystem.Create("ServerActorSystem", ConfigurationLoader.Load()));

            //leader actor
            services.AddSingleton<LeaderActorProvider>(provider =>
            {
                var actorSystem = provider.GetService<ActorSystem>();

                var leaderActor = actorSystem.ActorOf(Props.Create(() => new LeaderActor()), "leader");

                return () => leaderActor;
            });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", info: new Swashbuckle.AspNetCore.Swagger.Info() {Title = "AkkaAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            lifetime.ApplicationStarted.Register( () => 
            {
                app.ApplicationServices.GetService<ActorSystem>();
            });

            lifetime.ApplicationStopping.Register(() => 
            {
                app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AkkaAPI V1");
            });


            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
