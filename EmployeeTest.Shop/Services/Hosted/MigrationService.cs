using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OliDemos.Shop.Model;
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
        private readonly ILogger<MigrationService> _logger;
        private readonly IServiceScopeFactory _serviceScope;

        public MigrationService(ILogger<MigrationService> logger, IServiceScopeFactory serviceScope)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested(); // check token
            var scope = _serviceScope.CreateScope();
            var backendDb = scope.ServiceProvider.GetService<ShopContext>();
            try
            {
                var pedding = await backendDb.MigrationPedding(cancellationToken);
                int count = await ApplyMigrations(backendDb, pedding, cancellationToken);
                bool seeded = await ApplySeed(backendDb, cancellationToken);

                _logger.LogInformation("Resume: {0} migrations\tApply Seed: {1}",
                    count,
                    seeded ? "yes" : "no");
            }
            catch (OperationCanceledException)
            {
                // ignore catch
            }
            catch (Exception err)
            {
                _logger.LogError("Exception is throwed: {0}", err.GetType().Name);
                _logger.LogError("Message: {0}", err.Message);
            }
            finally
            {
                _logger.LogInformation("finalziando el servicio de migraciones");
                await backendDb.DisposeAsync();
                scope.Dispose();
            }
        }

        private async Task<bool> ApplySeed(ShopContext backendDb, CancellationToken cancellationToken)
        {
            bool applied = false;
            bool hasUser = await backendDb.Set<User>().AnyAsync(cancellationToken);
            if (hasUser)
            {
                // do
                applied = true;
            }
            return applied;
        }

        private async Task<int> ApplyMigrations(ShopContext backendDb, IEnumerable<string> pedding, CancellationToken cancellationToken)
        {
            int count;
            /// si hay migraciones pendientes entonces se lanza el proceso de crear migraciones
            if ((count = pedding.Count()) > 0)
            {
                _logger.LogInformation("Hay {0} cantidades de migraciones", count);
                _logger.LogInformation("Ejecutando las migraciones...");

                // finalmente se ejecuta las migraciones
                try
                {
                    await backendDb.MigrateAsync(cancellationToken);
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

            return count;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
