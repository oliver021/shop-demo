using OliDemos.Shop.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics;

namespace OliDemos.Shop.Services
{
    public class ShopContext : IdentityDbContext<User, IdentityRole<ulong>, ulong>
    {
        public ShopContext(DbContextOptions options) : base(options)
        {
        }

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
        }
    }
}
