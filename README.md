# CoreLoggerSuite
A powerfull utility for ASP Net Core MVC application to manage logs.

CoreLogger is a easy tool for logging form Core Application.
CoreLogger can log in txt file, SQLite db file or MS SQL Server, at same time!

You can use at runtime with CoreLogger class, or in ASP Core application with CoreLoggerMiddelware.

# How to use

Configure the option class in StartUp.CofigureServices:

        public void ConfigureServices(IServiceCollection services)
        {

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


Indicate the app to use the middleware (optional) an the inspector (optional, route "/corelogger"):

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
