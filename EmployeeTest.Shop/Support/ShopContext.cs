using OliDemos.Shop.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace OliDemos.Shop.Services
{
    public class ShopContext : DbContext
    {

        public ShopContext([NotNull] DbContextOptions options) : base(options)
        { /* not do any action */}

        /// <summary>
        /// Realiza la migracion hasta la ultima version del esquema
        /// </summary>
        /// <returns></returns>
        public async virtual Task MigrateAsync(CancellationToken cancellationToken = default)
            => await Database.MigrateAsync(cancellationToken);

        /// <summary>
        /// Regresa una lista de migraciones aplicadas
        /// </summary>
        /// <returns></returns>
        public async virtual Task<IEnumerable<string>> MigrationApplied(CancellationToken cancellationToken = default)
            => await Database.GetAppliedMigrationsAsync(cancellationToken);

        /// <summary>
        /// Regresa una lista de migraciones pendientes
        /// Util pa saber si se debe efectuar o no un actualizacion
        /// del esquema a traves del commando MigrateAsync
        /// </summary>
        /// <returns></returns>
        public async virtual Task<IEnumerable<string>> MigrationPedding(CancellationToken cancellationToken = default)
            => await Database.GetPendingMigrationsAsync(cancellationToken);


        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);
            builder.Entity<Product>();
            builder.Entity<Order>();
            builder.Entity<User>();
        }
    }
}
