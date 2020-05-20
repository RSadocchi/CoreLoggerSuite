using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLogger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestApp
{
    public class Startup
    {
        public readonly IConfiguration Configuration;
        readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            /**
             * This is the configuration to be able to use CoreLogger in the application
             */
            services.ConfigureCoreLogger(o =>
            {
                /**
                 * Base and Middleware
                 */
                o.MinLevel = Microsoft.Extensions.Logging.LogLevel.Trace;
                o.File_FolderPath = System.IO.Path.Combine(_environment.WebRootPath, "App_Log");
                o.SQLite_FullPath = System.IO.Path.Combine(_environment.WebRootPath, "Log.db3");
                //o.SQL_ConnectionString = "To use SQL indicate here the connection string";

                /**
                 * Inspector
                 */
                o.Title = "CoreLoggerInspector Test";
                o.Description = "CoreLoggerInspector Test MVC App";
                o.Contacts = new Contact[]
                {
                    new Contact()
                    {
                        Name = "Jhon Doe",
                        Role = "Tech Support",
                        Mobile = "123456789",
                        Email = "jdoe@example.com"
                    },
                    new Contact()
                    {
                        Name = "Adam Smith",
                        Role = "Tech Support",
                        Mobile = "123456789",
                        Email = "asmith@example.com"
                    }
                };
                o.Environments = new EnvConfiguration[]
                {
                    new EnvConfiguration()
                    {
                        EnvironmentName = "Development",
                        UseAuthentication = new Credentials()
                        {
                            Username = "admin",
                            Password = "password",
                            Expiring = TimeSpan.FromMinutes(20)
                        }
                    },
                    new EnvConfiguration()
                    {
                        EnvironmentName = "Production",
                        DenyEnvironmet = true
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /**
             * This indicates to application to use the miidleware to automatically log errors (always), 
             * warnings and traces (optional based on the minimum level set)
             */
            app.UseMiddleware<CoreLoggerMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            /**
             * This loads the necessary views to view the logs into the application, 
             * using "/corelogger" route
             */
            app.UseCoreLoggerInspector();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
