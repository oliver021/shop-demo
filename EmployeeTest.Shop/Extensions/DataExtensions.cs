using EmployeeTest.Shop.Components.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OliDemos.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataExtensions
    {
        public static IServiceCollection AddServiceCollection(
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

            services.AddDbContext<DbContext, ShopContext>(config => {
                var dbSetting = configuration.GetSection("Database")
                .Get<DatabaseSetting>();

                config.UseMySql(dbSetting.Connection, MySql => {
                    MySql.EnableIndexOptimizedBooleanColumns(true);
                    MySql.ServerVersion(Version.Parse(dbSetting.Version), dbSetting.UseMariaDb ? ServerType.MariaDb : ServerType.MySql);
                    
                    // retry method to ensure that request is executed
                    if(dbSetting.Resilent) MySql.EnableRetryOnFailure();


                });
            });
            return services;
        }
    }
}
