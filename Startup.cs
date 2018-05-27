using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExploreCaliforna.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ExploreCaliforna
{
    public class Startup
    {
        public readonly IConfigurationRoot configuration;

        public Startup(IHostingEnvironment env)
        {
            configuration  = new ConfigurationBuilder().
                AddEnvironmentVariables()
               .AddJsonFile(env.ContentRootPath + "/config.json")
               .AddJsonFile(env.ContentRootPath + "/config.dev.json", true)
                .Build();

       //     var host = new WebHostBuilder()
       //.UseContentRoot(Directory.GetCurrentDirectory())
       //.UseKestrel()
       //.UseIISIntegration() // Necessary for Azure.
       //.UseStartup<Program>()
       //.Build();

       //     host.Run();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<nugetstatiktext.DosyaOkuyucu>();
            services.AddTransient<FormattingService>();
            //services.AddTransient<SpeacialDataContext>();
            services.AddDbContext<BlogDataContext>( options =>
            {
                var connectionString = configuration.GetConnectionString("BlogDataContext");
                options.UseSqlServer(connectionString);
            }
                
                );
            services.AddDbContext<SpeacialDataContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("SpeacialDataContext");
                options.UseSqlServer(connectionString);
            });

             services.AddDbContext<IdentityDataContext>(options =>
             {
                 var connectionString = configuration.GetConnectionString("IdentityDataContext");
                 options.UseSqlServer(connectionString);
             }

                 );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler("/error.html");

            //var configuration = new ConfigurationBuilder().
            //    AddEnvironmentVariables()
            //   .AddJsonFile(env.ContentRootPath + "/config.json")
            //   .AddJsonFile(env.ContentRootPath + "/config.dev.json",true)
            //    .Build();

            //if (env.IsDevelopment())
            if (configuration.GetValue<bool>("DevOrtamiAyar"))
            {
                // Yazılımcı icin var
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/hata"))
                {
                    throw new Exception("Hata !!!!");
                }
                await next();
            });


            //app.Use(async (context,next) =>
            //{
            //    if (context.Request.Path.Value.StartsWith("/start"))
            //    {
            //        await context.Response.WriteAsync("Start Day!");
            //    }
            //    await next();
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello Ziraat Teknoloji!");
            //});

            app.UseFileServer();


            app.UseAuthentication();

             app.UseMvc( routes => {
                routes.MapRoute("Default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
