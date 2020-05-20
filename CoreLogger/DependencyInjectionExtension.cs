using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreLogger
{
    public static partial class DependencyInjectionExtension
    {
        public static IServiceCollection ConfigureCoreLogger(this IServiceCollection services, Action<CoreLoggerConfiguration> configuration)
        {
            services.Configure(configuration);
            services.AddSingleton<ICoreLogger, CoreLogger>();
            services.AddSingleton<IInspectorService, InspectorService>();
            services.AddRazorPages();
            return services;
        }

        public static void UseCoreLoggerInspector(this IApplicationBuilder app) => app.UseEndpoints(endpoints => endpoints.MapRazorPages());
    }
}
