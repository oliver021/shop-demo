using Microsoft.AspNetCore.Identity;
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
        private const string CreateUserMsg = "created user: ";
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
                bool seeded = await ApplySeed(scope, backendDb, cancellationToken);

                _logger.LogInformation("Resume: {0} migrations\tApply Seed: {1}",
                    count,
                    seeded ? "yes" : "no");
            }
            catch(InvalidOperationException err) 
            when (err.Message.StartsWith(CreateUserMsg))
            {
                _logger.LogCritical("The user creation is required and fails user creation");
                _logger.LogError(err.Message);
            }
            catch (OperationCanceledException)
            {
                // ignore catch because the operation was canceled by user request
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

        private async Task<bool> ApplySeed(IServiceScope scope, ShopContext backendDb, CancellationToken cancellationToken)
        {
            bool appliedRole = false;
            bool appliedUser = false;
            appliedRole = await SeedRoles(backendDb, appliedRole, cancellationToken);
            appliedUser = await SeedUsers(scope, backendDb, appliedUser, cancellationToken);
            return appliedRole || appliedUser;
        }

        private static async Task<bool> SeedUsers(IServiceScope scope, ShopContext backendDb, bool appliedUser, CancellationToken cancellationToken)
        {
            bool hasUser = await backendDb.Set<User>().AnyAsync(cancellationToken);

            if (!hasUser)
            {
                var identity = scope.ServiceProvider.GetService<UserManager<User>>();
                const string __pass = "secret";

                IdentityResult result;

                var userAdmin = new User
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@email.domain.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Shop",
                };

                var userClient = new User
                {
                    UserName = "client",
                    NormalizedUserName = "CLIENT",
                    Email = "client@email.domain.com",
                    EmailConfirmed = true,
                    FirstName = "Client",
                    LastName = "Shop",
                };

                var userSeller = new User
                {
                    UserName = "seller",
                    NormalizedUserName = "SELLER",
                    Email = "client@email.domain.com",
                    EmailConfirmed = true,
                    FirstName = "Client",
                    LastName = "Shop",
                };

                result = await identity.CreateAsync(userAdmin, __pass);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(CreateUserMsg + userAdmin.Email);
                }

                result = await identity.CreateAsync(userClient, __pass);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(CreateUserMsg + userClient.Email);
                }

                result = await identity.CreateAsync(userSeller, __pass);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(CreateUserMsg + userClient.Email);
                }

                await identity.AddToRoleAsync(userAdmin, "Admin");
                await identity.AddToRoleAsync(userClient, "Client");
                await identity.AddToRoleAsync(userSeller, "Seller");
                appliedUser = true;
            }

            return appliedUser;
        }

        private static async Task<bool> SeedRoles(ShopContext backendDb, bool appliedRole, CancellationToken cancellationToken)
        {
            var roles = backendDb.Roles;
            bool hasRoleAdmin = await roles.AnyAsync(r => r.Name == "Admin", cancellationToken);
            bool hasRoleClient = await roles.AnyAsync(r => r.Name == "Client", cancellationToken);
            bool hasRoleSeller = await roles.AnyAsync(r => r.Name == "Seller", cancellationToken);

            if (!hasRoleAdmin)
            {
                backendDb.Add(new IdentityRole<ulong>
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });
                appliedRole = true;
            }

            if (!hasRoleClient)
            {
                backendDb.Add(new IdentityRole<ulong>
                {
                    Name = "Client",
                    NormalizedName = "CLIENT"
                });
                appliedRole = true;
            }

            if (!hasRoleSeller)
            {
                backendDb.Add(new IdentityRole<ulong>
                {
                    Name = "Seller",
                    NormalizedName = "SELLER"
                });
                appliedRole = true;
            }

            if (appliedRole)
            {
                await backendDb.SaveChangesAsync(cancellationToken);
            }

            return appliedRole;
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
