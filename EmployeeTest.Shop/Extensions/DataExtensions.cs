using EmployeeTest.Shop.Components.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OliDemos.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using EmployeeTest.Shop.Services.Hosted;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var dbSetting = configuration.GetSection("Database")
                .Get<DatabaseSetting>();

            services.AddDbContext<DbContext, ShopContext>(config => {
                

                ServerVersion serverVersion =  ServerVersion.FromString(dbSetting.Version);
                
                config.UseMySql(dbSetting.Connection, serverVersion, MySql => {
                    MySql.EnableIndexOptimizedBooleanColumns(true);
                    
                    // retry method to ensure that request is executed
                    if(dbSetting.Resilent) MySql.EnableRetryOnFailure();
                });
            });

            // simple alias
            services.AddScoped<ShopContext>(p => (ShopContext)p.GetService<DbContext>());

            if (dbSetting.MigrationService)
            {
                // deploy this service
                services.AddHostedService<MigrationService>();
            }

            // data services for extending repositories
            services.AddScoped<ProductService>();
            services.AddScoped<OrderService>();
            return services;
        }
    }
}
