using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OliDemos.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeTest.Shop.Services.Hosted
{
    public class MigrationService : IHostedService
    {
        private ILogger<MigrationService> _logger;
        private readonly IServiceScopeFactory _serviceScope;

        public MigrationService(ILogger<MigrationService> logger, IServiceScopeFactory serviceScope)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceScope.CreateScope();
            var backendDb = scope.ServiceProvider.GetService<ShopContext>();
            var pedding = await backendDb.MigrationPedding();

            int count;

            /// si hay migraciones pendientes entonces se lanza el proceso de crear migraciones
            if ((count = pedding.Count()) > 0)
            {
                _logger.LogInformation("Hay {0} cantidades de migraciones", count);
                _logger.LogInformation("Ejecutando las migraciones...");

                // finalmente se ejecuta las migraciones
                try
                {
                    await backendDb.MigrateAsync();
                    _logger.LogInformation("Realizacion de las migraciones correctamente", count);
                }
                catch (Exception err)
                {
                    _logger.LogError(err.Message);
                    if (err.InnerException != null)
                    {
                        _logger.LogError(err.InnerException.Message);
                    }
                    _logger.LogCritical("No se puede seguir con la tarea...");
                }
            }
            else
            {
                _logger.LogInformation("No hay migraciones pendientes...");
            }

            await backendDb.DisposeAsync();
            scope.Dispose();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
