using EmployeeTest.Shop.Components.Attributes;
using Microsoft.Extensions.Configuration;
using NSwag;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExtraExtensions
    {
        /// <summary>
        /// this method discover all setting in this project
        /// to bind with configuration file
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection PrepareSettings(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Type attr = typeof(SettingAttribute);

            Assembly.GetCallingAssembly()
                .GetExportedTypes()
                .Where(t => t.IsDefined(attr, false))
                .ToList()
                .ForEach(t => {
                    var sett = t.GetCustomAttribute<SettingAttribute>();
                    var config = configuration.GetSection(sett.KeyConfig);
                    if (config.Exists())
                    {
                        services.AddSingleton(t, delegate {
                            var result =  t.GetConstructor(Type.EmptyTypes).Invoke(null);
                            config.Bind(result);
                            return result;
                        });
                    }
                });
            return services;
        } 

        public static IServiceCollection AddSwaggerSupport(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            // add Open API
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "System Data API";
                    document.Info.Description = "Este es el API de acceso a datos de la tienda";
                };

                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("apiKey"));
                config.DocumentProcessors.Add(new SecurityDefinitionAppender("apiKey", new OpenApiSecurityScheme()
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "key",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Token Maestro de que pueba que tienes privilegios de admnistracion",
                    Flow = OpenApiOAuth2Flow.AccessCode
                }));
            });
            return services;
        }
    }
}
